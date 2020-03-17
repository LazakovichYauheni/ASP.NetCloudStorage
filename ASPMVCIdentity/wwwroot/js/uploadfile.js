function createXMLHttp() {
    if (typeof XMLHttpRequest != "undefined") {
        return new XMLHttpRequest();
    } else if (window.ActiveXObject) {
        var aVersions = ["MSXML2.XMLHttp.5.0", "MSXML2.XMLHttp.4.0", "MSXML2.XMLHttp.3.0", "MSXML2.XMLHttp", "Microsoft.XMLHttp"];
        for (var i = 0; i < aVersions.length; i++) {
            try {
                var oXmlHttp = new ActiveXObject(aVersions[i]);
                return oXmlHttp;
            } catch (oError) {
                //void
            }
        }
    }
    throw new Error("XMLHttp object could not be created.");
}
//used for posting "multipart/form-data"
function buildFormData(form_object) {
    var fd = new FormData();
    for (var i = 0; i < form_object.elements.length; i++) {
        if (form_object.elements[i].name != null && form_object.elements[i].name != "") {
            if (form_object.elements[i].type == "checkbox") {
                if (form_object.elements[i].checked) {
                    fd.append(form_object.elements[i].name, form_object.elements[i].value)
                }
            }
            else if (form_object.elements[i].type == "file") {
                for (var j = 0; j < form_object.elements[i].files.length; j++) {
                    fd.append(form_object.elements[i].name,
                        form_object.elements[i].files[j],
                        form_object.elements[i].files[j].name)
                }
            }
            else {
                fd.append(form_object.elements[i].name, form_object.elements[i].value)
            }
        }
    }
    return fd;
}
function submitForm(oform) {
    if (window.FormData !== undefined) {
        document.getElementById("SubmitButton").disabled = true;
        var formData = buildFormData(oform);
        var xmlobj = createXMLHttp();
        xmlobj.onreadystatechange = function () {
            if (xmlobj.readyState == 4) {
                if (xmlobj.status == 200) {
                    document.getElementById("divResponse").innerHTML = xmlobj.responseText;
                    document.getElementById("SubmitButton").disabled = false;
                }
                else {
                    throw new Error("Error: " + xmlobj.status + ": " + xmlobj.statusText);
                }
            }
        };
        xmlobj.open("post", oform.action, true);
        xmlobj.send(formData);
    }
    else {
        alert("This browser does not support posting files with HTML5 and AJAX.");
    }
    return false;
}