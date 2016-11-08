<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TramitacaoArquivoVM>" %>
<div>
	<div class="block box">
		<div class="block">
			<div class="coluna99">
				<label for="TramitacaoArquivo.Nome">
					Nome *</label>
				<%= Html.TextBox("TramitacaoArquivo.Nome", Model.TramitacaoArquivo.Nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNome", maxlength = "100" }))%>
			</div>
		</div>
		<div class="block">
			<div class="coluna59">
				<label for="TramitacaoArquivo.SetorId">
					Setor *</label>
				<%= Html.DropDownList("TramitacaoArquivo.SetorId", Model.Setores, ViewModelHelper.SetaDisabled(Model.IsVisualizar || !Model.PodeEditarSetor, new { @class = "text ddlSetor" }))%>				
			</div>
			<div class="coluna37 prepend2">
				<label for="TramitacaoArquivo.TipoId">Tipo *</label>
				<%= Html.DropDownList("TramitacaoArquivo.TipoId", Model.Tipos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipo" }))%>
			</div>
		</div>
	</div>
	<fieldset class="block box fsAdicionarAtividade">
		<legend>Localização</legend>
		<div class="divConteudoEstante associarMultiplo">
			<div class="asmItens">
				<% foreach (var estante in Model.EstantesVM) { %>
					<div class="asmItemContainer" style="border: 0px">
						<% Html.RenderPartial("Estante", estante); %>
					</div>
				<% } %>
			</div>
			<%if (!Model.IsVisualizar) { %>
			<div class="coluna100 negtTop15" style="border: 0px;">
				<p class="floatRight"><button class="btnAsmAdicionar btAdicionar btnaddAtivSol " title="Adicionar Estante">Estante</button></p>
			</div>
			<div class="asmItemTemplateContainer asmItemContainer hide" style="border: 0px">
				<% Html.RenderPartial("Estante", Model.EstanteTemplate); %>
			</div>
			<%} %>
		</div>
	</fieldset>

	<!-- Inicio Processo/Documento -->
	<fieldset class="block box divProcessoSituacaos">
		<legend>Processo / Documento</legend>
		<div class="block">
			<div class="asmConteudoInterno">
				<div class="block">
					<label>
						Situação da atividade solicitada na qual o processo será arquivado.</label>
				</div>
				<div class="divSituacoes block">
					<%for (int i = 0; i < Model.SituacoesProcessoAtividade.Count; i++)
					{
					 bool selecionado = (Model.TramitacaoArquivo.ProtocoloSituacao.HasValue && (Model.SituacoesProcessoAtividade[i].Codigo & Model.TramitacaoArquivo.ProtocoloSituacao.Value) > 0);
					%>
					<div class="coluna21">
						<label class="labelCheckBox">
							<input class="ckProcessoSituacao <%= Model.IsVisualizar ? " disabled" : "" %>" type="checkbox" title="<%= Model.SituacoesProcessoAtividade[i].Texto %>" <%= Model.IsVisualizar ?" disabled=\"disabled\"" : "" %>
								value="<%= Model.SituacoesProcessoAtividade[i].Codigo %>" <%= selecionado ? "checked=\"checked\"" : ""  %> />
							<%= Model.SituacoesProcessoAtividade[i].Texto %></label>
					</div>
					<% } %>
				</div>
			</div>
		</div>
	</fieldset>
	<fieldset class="block box divDocumentoSituacaos hide">
		<legend>Documento</legend>
		<div class="block">
			<div class="asmConteudoInterno">
				<div class="block">
					<label>
						Situação da atividade solicitada na qual o documento será arquivado.</label>
				</div>
				<div class="divSituacoes block">
					<%for (int i = 0; i < Model.SituacoesDocumentoAtividade.Count; i++)
					{
						bool selecionado = (Model.TramitacaoArquivo.ProtocoloSituacao.HasValue && (Model.SituacoesDocumentoAtividade[i].Codigo & Model.TramitacaoArquivo.ProtocoloSituacao.Value) > 0);
					%>
					<div class="coluna21">
						<label class="labelCheckBox">
							<input class="ckDocumentoSituacao <%= Model.IsVisualizar ? " disabled" : "" %>" type="checkbox" title="<%= Model.SituacoesDocumentoAtividade[i].Texto %>"  <%= Model.IsVisualizar ?" disabled=\"disabled\"" : "" %>
								value="<%= Model.SituacoesDocumentoAtividade[i].Codigo %>" <%= selecionado ? "checked=\"checked\"" : ""  %> />
							<%= Model.SituacoesDocumentoAtividade[i].Texto %></label>
					</div>
					<% } %>
				</div>
			</div>
		</div>
	</fieldset>
</div>
