<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<!-- REDIRECIONAMENTO ========================================================== -->
<div class="redirecinamento block containerAcoes hide">
	<h5>O que deseja fazer agora?</h5>
	<p class="hide">#DESCRICAO</p>

	<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
		<input type="hidden" class="hdnIdAcao" value="<%= Request.Params["acaoId"].ToString() %>" />
	<%}%>
	
	<div class="coluna100 margem0 divAcoesContainer">
		<p class="floatLeft margem0 append1"><button title="[title]" class="btnTemplateAcao hide">[ACAO]</button></p>
		<div class="containerBotoes"></div>
	</div>
</div><!-- .redirecinamento -->
<!-- ========================================================================= -->