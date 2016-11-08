<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<!-- Ativa o Intellisence -->
<% if (false) {%>
	<script type="text/javascript" language="javascript" src="../../Scripts/Lib/JQuery/jquery-1.4.1-vsdoc.js" ></script>
<% }%>

<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/Lib/FileUpload/jquery.fileupload.js") %>" ></script>
<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/Lib/FileUpload/jquery.fileupload-ui.js") %>" ></script>

<link href="<%= Url.Content("~/Content/_css/jquery.fileupload-ui.css") %>" rel="stylesheet" type="text/css" media="all">	

<form id="file_upload" action="<%= Url.Action("arquivo", "arquivo") %>" method="POST" enctype="multipart/form-data">
	<input type="file" name="file" multiple>
	<button>Upload</button>
	<div>Upload files</div>
</form>
<table id="files"></table>

<script>
	/*global $ */
	$(function () {
		$('#file_upload').fileUploadUI({
			uploadTable: $('#files'),
			downloadTable: $('#files'),
			buildUploadRow: function (files, index) {
				return $('<tr><td>' + files[index].name + '<\/td>' +
                    '<td class="file_upload_progress"><div><\/div><\/td>' +
                    '<td class="file_upload_cancel">' +
                    '<button class="ui-state-default ui-corner-all" title="Cancel">' +
                    '<span class="ui-icon ui-icon-cancel">Cancel<\/span>' +
                    '<\/button><\/td><\/tr>');
			},
			buildDownloadRow: function (file) {
				return $('<tr><td>' + file.name + '<\/td><\/tr>');
			},
			onLoad: function (event, files, index, xhr, handler) {
				if (typeof xhr.responseText !== 'undefined') {
					alert(xhr.responseText);
				} else {
					alert(xhr.contents().text());
				}
			}
		});
	});
</script> 