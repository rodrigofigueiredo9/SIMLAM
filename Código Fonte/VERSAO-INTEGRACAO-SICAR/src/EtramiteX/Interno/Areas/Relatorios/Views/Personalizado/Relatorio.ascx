<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>

<input type="hidden" class="jsonCnfRelatorio" value="<%: Model.ConfiguracaoRelatorioJSON %>" />

<!-- ========================================================================= -->
<div class="block margem0">
	<div class="wizBar coluna100">
		<ul>
			<li class="ativo"><span>Opções</span></li>
			<li class=""><span>Ordenar Colunas</span></li>
			<li class=""><span>Ordenar Valores</span></li>
			<li class=""><span>Filtros</span></li>
			<li class=""><span>Sumarizar</span></li>
			<li class=""><span>Dimensionar</span></li>
			<li class=""><span>Agrupar</span></li>
			<li class=""><span>Salvar</span></li>
		</ul>
	</div><!-- .wizBar -->
</div>
<!-- ========================================================================= -->

<!-- navegaçâo da wizard ========================================================================= -->
<div class="block box">
	<div class="coluna25 floatRight alinhaDireita">
		<button class="btnAvancar">Avançar</button>
	</div>	<div class="coluna35 floatRight alinhaDireita hide">		<button class="btnFinalizar">Salvar e Finalizar</button>	</div>
	<div class="coluna25 hide">
		<button class="btnVoltar">Voltar</button>
	</div>
</div>
<!-- ============================================================================================ -->

<input type="hidden" class="hdnRelatorioId" value="<%= Model.Id %>" />
<div class="conteudoRelatorio">
	<% Html.RenderPartial("Opcoes", Model); %>
</div>

<!-- navegaçâo da wizard ========================================================================= -->
<div class="block box margemDTop">
	<div class="coluna25 floatRight alinhaDireita">
		<span class="cancelarCaixaDir">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		<button class="btnAvancar">Avançar</button>
	</div>	<div class="coluna35 floatRight alinhaDireita hide">
		<span class="cancelarCaixaDir">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		<button class="btnFinalizar">Salvar e Finalizar</button>
	</div>
	<div class="coluna25 hide">
		<button class="btnVoltar">Voltar</button>
	</div>
</div>
<!-- ============================================================================================ -->