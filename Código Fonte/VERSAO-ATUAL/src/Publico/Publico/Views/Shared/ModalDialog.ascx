<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<!-- MODAL =================================================================== -->
<!-- Sempre colocar o código da Modal logo após a tag BODY -->
<div class="fundoModal hide modalTemplate" >
	<div class="modalBranco">
		<div class="block">
			<a class="fecharModal fecharModalOnClick" title="Fechar Mensagem">Fechar Mensagem</a>
		</div>
		<div class="boxModal">
			<div class="modalCarregando box boxModal">
				<!-- TEXTO E ANIMAÇÃO DE CARREGANDO =================================================================== -->
				<img src="<%= Url.Content("~/Content/_img/loader_branco.gif") %>" width="32" height="32" class="loaderCinza" alt="carregando">
				<p class="loaderTxtBranco">Carregando, por favor aguarde.</p>
				<!-- ========================================================================= -->
			</div>
			<div class="modalContent hide">
			</div>
		</div>
		<div class="modelBotoes block hide">
			<input type="submit" value="Ok" class="btnModalOk floatLeft" />
			<span class="btnModalCancelar cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar">Cancelar</a></span>
		</div>
	</div>
</div>
<!-- ========================================================================= -->

<div class="dialogContainer">
</div>
