<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Tecnomapas.Blocos.Etx.ModuloValidacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMValidacao" %>

<% if (!String.IsNullOrEmpty(Request.Params["msg"]))
   {
	   Validacao.QueryParamDeserializer(Request.Params["msg"]);
   }
 %>

<div class="block mensagemContent">
<% if (Validacao.Erros.Count > 0){%>
	<% List<eTipoMensagem> tipoMsgOrdem = new List<eTipoMensagem>() {
			eTipoMensagem.Erro,
			eTipoMensagem.Advertencia,
			eTipoMensagem.Sucesso,
			eTipoMensagem.Informacao};

	List<string> tipoMsgCssOrdem = new List<string>() {
			"erro",
			"alerta",
			"sucesso",
			"info"};
		%>

	<% foreach (eTipoMensagem itemTipo in tipoMsgOrdem){%>

		<!-- MENSAGEM DE <%= itemTipo.ToString() %> ========================================================== -->
		<% if ( ValidacaoVM.TemMensagem(itemTipo)) {%>
		<div class="mensagemSistema <%= tipoMsgCssOrdem[tipoMsgOrdem.IndexOf(itemTipo)] %>">
			<div class="textoMensagem">
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
		</div><!-- .mensagemSistema -->
		<%} %>
		<!-- ========================================================================= -->
		
	<%} %>

<% if (Validacao.Erros.Any(x => x.Tipo == eTipoMensagem.Advertencia))
{%>
<script type="text/javascript" language="javascript">
<!--

	$(window).load(function () {

		<% foreach (var item in Validacao.Erros.Where(x => x.Tipo == eTipoMensagem.Advertencia && !String.IsNullOrEmpty(x.Campo) ).GroupBy(x => x.Campo) )
		{ %>
			$("#<%= item.Key %>").addClass("erroCampo");
		<%} %>
	});

-->
</script>
<%  } %>

<%  } %>
</div>