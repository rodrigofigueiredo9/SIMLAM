<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Tecnomapas.Blocos.Etx.ModuloValidacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMValidacao" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Data" %>

<% if (!String.IsNullOrEmpty(Request.Params["msg"]))
   {
	   Validacao.QueryParamDeserializer(Request.Params["msg"]);
   }
 %>

<div class="mensagemSistemaHolder">
<% if (Validacao.Erros.Count > 0){%>
	<% List<eTipoMensagem> tipoMsgOrdem = new List<eTipoMensagem>() {
			eTipoMensagem.Erro,
			eTipoMensagem.Advertencia,
			eTipoMensagem.Sucesso,
			eTipoMensagem.Informacao,
			eTipoMensagem.Ajuda};

	List<string> tipoMsgCssOrdem = new List<string>() {
			"erro",
			"alerta",
			"sucesso",
			"info",
			"ajuda"};
		%>

	<% foreach (eTipoMensagem itemTipo in tipoMsgOrdem){%>

		<!-- MENSAGEM DE <%= itemTipo.ToString() %> ========================================================== -->
		<% if ( ValidacaoVM.TemMensagem(itemTipo)) {%>
		<div class="mensagemSistema <%= tipoMsgCssOrdem[tipoMsgOrdem.IndexOf(itemTipo)] %>">
			<div class="textoMensagem <%: (itemTipo == eTipoMensagem.Ajuda)? "textoAjuda":"" %>">
				<a class="fecharMensagem" title="Fechar Mensagem">Fechar Mensagem</a>
				<p>Mensagem do Sistema</p>

				<ul>
					<% foreach (var item in Validacao.Erros.Where(x => x.Tipo == itemTipo))
					   {%>
						   <li><%= Html.Encode(item.Texto)%></li>
					<%} %>
				</ul>
			</div>
			<% if (ValidacaoVM.ExibirMais(itemTipo)){ %>
			<a class="linkVejaMaisMensagens" title="Clique aqui para ver mais detalhes desta mensagem">Clique aqui para ver mais detalhes desta mensagem</a>
			<%} %>

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

		</div><!-- .mensagemSistema -->
		<%} %>
		<!-- ========================================================================= -->

	<%} %>

<% if (Validacao.Erros.Any(x => x.Tipo == eTipoMensagem.Advertencia))
{%>
<script>

	$(window).load(function () {

		<% foreach (var item in Validacao.Erros.Where(x => x.Tipo == eTipoMensagem.Advertencia && !String.IsNullOrEmpty(x.Campo) ).GroupBy(x => x.Campo) )
		{ %>
			$("#<%= item.Key %>").addClass("erroCampo");
		<%} %>
	});

</script>
<%  } %>

<%  } %>

</div>
