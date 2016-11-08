<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EnquadramentoVM>" %>

<div class="FiscalizacaoEnquadramentoContainer">
	<input type="hidden" class="hdnEnquadramentoId" value="<%:Model.Entidade.Id %>" />
	<input type="hidden" class="hdnIsSalvo" value="false" />
	<fieldset class="box">
		<legend>Embasamento legal</legend>
		<div class="divArtigos">
			<%foreach (Artigo artigo in Model.Entidade.Artigos){%>
				<fieldset class="fsArtigo boxBranca">
					<input type="hidden" class="hdnIdentificador" value="<%:artigo.Identificador %>" />
					<input type="hidden" class="hdnArtigoId" value="<%:artigo.Id %>" />

					<div class="block">
						<div class="coluna10 append2">
							<label for="Enquadramento_ArtigoTexto<%:artigo.Identificador%>">Artigo *</label>
							<%= Html.TextBox("Enquadramento.ArtigoTexto" + artigo.Identificador, artigo.ArtigoTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArtigoTexto", @maxlength = "5" }))%>
						</div>

						<div class="coluna20 append2">
							<label for="Enquadramento_ArtigoParagrafo<%:artigo.Identificador%>">Item/Parágrafo</label>
							<%= Html.TextBox("Enquadramento.ArtigoParagrafo" + artigo.Identificador, artigo.ArtigoParagrafo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArtigoParagrafo", @maxlength = "20" }))%>
						</div>

						<div class="coluna25 append2">
							<label for="Enquadramento_ArtigoCombinado<%:artigo.Identificador%>">Combinado com artigo</label>
							<%= Html.TextBox("Enquadramento.ArtigoCombinado" + artigo.Identificador, artigo.CombinadoArtigo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCombinadoArtigo", @maxlength = "20" }))%>
						</div>

						<div class="coluna20">
							<label for="Enquadramento_ArtigoCombinadoTexto<%:artigo.Identificador%>">Item/Parágrafo</label>
							<%= Html.TextBox("Enquadramento.ArtigoCombinadoTexto" + artigo.Identificador, artigo.CombinadoArtigoParagrafo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCombinadoArtigoParagrafo", @maxlength = "20" }))%>
						</div>
					</div>

					<div class="block">
						<div class="coluna84">
							<label for="Enquadramento_Enquadramento_DaDo">Da/Do (Citar norma legal: lei, decreto, resolução, portaria, etc) *</label>
							<%= Html.TextBox("Enquadramento.Enquadramento_DaDo" + artigo.Identificador, artigo.DaDo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDaDo", @maxlength = "100" }))%>
						</div>
					</div>
				</fieldset>
			<%} %>
		</div>
	</fieldset>
</div>