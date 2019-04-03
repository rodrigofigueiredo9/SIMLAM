<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block boxBranca">
	<legend class="titFiltros">Responsabilidade Técnica</legend>

	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela elaboração da Declaração de Dispensa de Licenciamento Ambiental de Barragem</legend>

		<div class="block">

			<div class="coluna30">
				<%=Html.Hidden("rtId", Model.Caracterizacao.responsaveisTecnicos[0].id, new { @class="hdnRTElaboracaoDeclaracaoId" })%>
				<label for="RTNome">Nome *</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[0].nome, ViewModelHelper.SetaDisabled(true, new { @class = "text txtRTElaboracaoDeclaracaoNome", maxlength = "80" })) %>
			</div>
			<div class="coluna30">
				<label for="RTProfissao">Profissão *</label>
				<%= Html.DropDownList("RT", Model.profissoesLst[0], ViewModelHelper.SetaDisabled(true, new { @class = "text ddlRTElaboracaoDeclaracaoProfissao" }))%>
			</div>
			<div class="coluna30">
				<label for="RTCREA">Registro CREA/ES *</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[0].registroCREA, ViewModelHelper.SetaDisabled(true, new { @class = "text txtRTElaboracaoDeclaracaoCREA", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna100">
				<div class="coluna65">
					<label for="RTNum">Número da ART de elaboração da Declaração de Dispensa de Licenciamento Ambiental de Barragem *</label>
				</div><br />
				<div class="coluna30">
					<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[0].numeroART, ViewModelHelper.SetaDisabled(true, new { @class = "text txtRTElaboracaoDeclaracaoNumero", maxlength = "14" })) %>
				</div>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela elaboração do projeto técnico ou do laudo de barragem construída</legend>

		<div class="block">
			<h5>Por orientação do Crea/ES somente estão aptos a serem responsáveis técnicos de barragens de terra engenheiros agrônomos,
				engenheiros agrícolas e engenheiros civis, e apenas engenheiros civis no caso de barragens de concreto ou mista.
				Demais profissionais somente serão aceitos com a apresentação de autorização específica do Crea/ES.
			</h5>
			<%=Html.Hidden("rtId", Model.Caracterizacao.responsaveisTecnicos[1].id, new { @class="hdnRTElaboracaoProjetoId" })%>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome *</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[1].nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar ||Model.Caracterizacao.responsaveisTecnicos[1].proprioDeclarante, new { @class = "text txtRTElaboracaoProjetoNome", maxlength = "80" })) %>
			</div>
			<div class="coluna30">
				<label for="RTProfissao">Profissão *</label>
				<%= Html.DropDownList("RT", Model.profissoesLst[1], ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[1].proprioDeclarante, new { @class = "text ddlRTElaboracaoProjetoProfissao" }))%>
			</div>
			<div class="coluna30">
				<label for="RTCREA">Registro CREA/ES *</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[1].registroCREA, ViewModelHelper.SetaDisabled(Model.IsVisualizar ||Model.Caracterizacao.responsaveisTecnicos[1].proprioDeclarante, new { @class = "text txtRTElaboracaoProjetoCREA", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="block arquivoRT <%=Model.Caracterizacao.responsaveisTecnicos[1].autorizacaoCREA.Id.GetValueOrDefault() > 0
					|| !Model.ProfissoesAutorizacao.Contains(Model.Caracterizacao.responsaveisTecnicos[1].profissao.Id) ? "" : "hide" %> ">
				<div class="coluna80 inputFileDiv">
					<label for="ArquivoTexto">Autorização do conselho de classe *</label>
					<% if(Model.Caracterizacao.responsaveisTecnicos[1].autorizacaoCREA.Id.GetValueOrDefault() > 0) { %>
					<div>
						<%= Html.ActionLink(ViewModelHelper.StringFit(Model.Caracterizacao.responsaveisTecnicos[1].autorizacaoCREA.Nome, 45), "Baixar", "Arquivo", new { @id = Model.Caracterizacao.responsaveisTecnicos[1].autorizacaoCREA.Id.GetValueOrDefault() }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.Caracterizacao.responsaveisTecnicos[1].autorizacaoCREA.Nome })%>
					</div>
					<% } %>
					<%= Html.TextBox("responsaveisTecnicos[1].autorizacaoCREA.Nome", null, new { readOnly = "true", @class = "text txtArquivoNome hide disabled", @disabled = "disabled" })%>
					<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Caracterizacao.responsaveisTecnicos[1].autorizacaoCREA.Nome) ? "" : "hide" %>">
					<input type="file" id="ArquivoId" class="inputFile" style="display: block; width: 100%" name="file" /></span>
					<input type="hidden" class="hdnArquivo" value="<%: Model.AutorizacaoJson%>" />
				</div>

				<div class="block ultima spanBotoes">
					<button type="button" class="inlineBotao btnArq <%= string.IsNullOrEmpty(Model.Caracterizacao.responsaveisTecnicos[1].autorizacaoCREA.Nome) ? "" : "hide" %>" title="Enviar anexo" onclick="BarragemDispensaLicenca.enviarArquivo('<%= Url.Action("arquivo", "arquivo") %>');">Enviar</button>
					<%if (!Model.IsVisualizar) {%>
					<button type="button" class="inlineBotao btnArqLimpar <%= string.IsNullOrEmpty(Model.Caracterizacao.responsaveisTecnicos[1].autorizacaoCREA.Nome) ? "hide" : "" %>" title="Limpar arquivo" ><span>Limpar</span></button>
					<%} %>
				</div>
			</div>
			<div class="coluna100">
				<div class="coluna65">
					<label for="DiametroTubulacaoVazaoMin">Número da ART de elaboração do projeto técnico / laudo de barragem construída *</label>
				</div><br />
				<div class="coluna30">
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[1].numeroART, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtRTElaboracaoProjetoNumero", maxlength = "14" })) %>
				</div>
			</div>
		</div>
	</fieldset>
	
	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela execução da barragem ou das adequações</legend>

		<div class="block">
			<label class ="coluna25">
				<%= Html.CheckBox("CopiaDeclarante", Model.Caracterizacao.responsaveisTecnicos[2].proprioDeclarante, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbCopiaDeclarante cbExecucaoBarragemCopiaDeclarante" }))%>
				Próprio Declarante
			</label> <br />
		</div>
		<div class="block">
			<%=Html.Hidden("rtId", Model.Caracterizacao.responsaveisTecnicos[2].id, new { @class="hdnRTExecucaoBarragemId" })%>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome </label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[2].nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[2].proprioDeclarante, new { @class = "text rtNome txtRTExecucaoBarragemNome", maxlength = "80" })) %>
			</div>
			<div class="coluna30">
				<label for="RTProfissao">Profissão </label>
				<%= Html.DropDownList("RT", Model.profissoesLst[2], ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[2].proprioDeclarante, new { @class = "text rtProfissao ddlRTExecucaoBarragemProfissao" }))%>
			</div>
			<div class="coluna30">
				<label for="RTCREA">Registro CREA/ES </label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[2].registroCREA, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[2].proprioDeclarante, new { @class = "text rtCREA txtRTExecucaoBarragemCREA", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna100">
				<div class="coluna65">
					<label for="DiametroTubulacaoVazaoMin">Número da ART de execução da barragem </label>
				</div><br />
				<div class="coluna30">
					<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[2].numeroART, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text rtNumero txtRTExecucaoBarragemNumero", maxlength = "14" })) %>
				</div>
			</div>
		</div>
	</fieldset>
		
	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela elaboração do estudo ambiental</legend>

		<div class="block">
			<%=Html.Hidden("rtId", Model.Caracterizacao.responsaveisTecnicos[3].id, new { @class="hdnRTElaboracaoEstudoAmbientalId" })%>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome *</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[3].nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[3].proprioDeclarante, new { @class = "text txtRTElaboracaoEstudoAmbientalNome", maxlength = "80" })) %>
			</div>
			<div class="coluna30">
				<label for="RTProfissao">Profissão *</label>
				<%= Html.DropDownList("RT", Model.profissoesLst[3], ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[3].proprioDeclarante, new { @class = "text ddlRTElaboracaoEstudoAmbientalProfissao" }))%>
			</div>
			<div class="coluna30">
				<label for="RTCREA">Registro CREA/ES *</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[3].registroCREA, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[3].proprioDeclarante, new { @class = "text txtRTElaboracaoEstudoAmbientalCREA", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna100">
				<div class="coluna65">
					<label for="DiametroTubulacaoVazaoMin">Número da ART de elaboração do estudo ambiental *</label>
				</div><br />
				<div class="coluna30">
					<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[3].numeroART, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtRTElaboracaoEstudoAmbientalNumero", maxlength = "14" })) %>
				</div>
			</div>
		</div>
	</fieldset>
			
	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela elaboração do Plano de Recuperação de Área Degradada referente à APP no entorno do reservatório</legend>
		
		<div class="block">
			<%=Html.Hidden("rtId", Model.Caracterizacao.responsaveisTecnicos[4].id, new { @class="hdnRTElaboracaoPlanoRecuperacaoId" })%>
			<label class ="coluna25">
				<%= Html.CheckBox("CopiaDeclarante", Model.Caracterizacao.responsaveisTecnicos[4].proprioDeclarante, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbCopiaDeclarante cbElaboracaoPlanoRecuperacaoCopiaDeclarante" }))%>
				Próprio Declarante
			</label> <br />
		</div>
		<div class="block">
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[4].nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[4].proprioDeclarante, new { @class = "text rtNome txtRTElaboracaoPlanoRecuperacaoNome", maxlength = "80" })) %>
			</div>
			<div class="coluna30">
				<label for="RTProfissao">Profissão</label>
				<%= Html.DropDownList("RT", Model.profissoesLst[4], ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[4].proprioDeclarante, new { @class = "text rtProfissao ddlRTElaboracaoPlanoRecuperacaoProfissao" }))%>
			</div>
			<div class="coluna30">
				<label for="RTCREA">Registro CREA/ES</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[4].registroCREA, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[4].proprioDeclarante, new { @class = "text rtCREA txtRTElaboracaoPlanoRecuperacaoCREA", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna100">
				<div class="coluna65">
					<label for="DiametroTubulacaoVazaoMin">Número da ART de elaboração do Plano de Recuperação de Área Degradada</label>
				</div><br />
				<div class="coluna30">
					<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[4].numeroART, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text rtNumero txtRTElaboracaoPlanoRecuperacaoNumero", maxlength = "14" })) %>
				</div>
			</div>
		</div>
	</fieldset>
				
	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela execução do Plano de Recuperação de Área Degradada referente à APP do entorno do reservatório</legend>
		
		<div class="block">
			<label class ="coluna25">
				<%= Html.CheckBox("CopiaDeclarante", Model.Caracterizacao.responsaveisTecnicos[5].proprioDeclarante, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbCopiaDeclarante cbExecucaoPlanoRecuperacaoCopiaDeclarante" }))%>
				Próprio Declarante
			</label> <br />
		</div>
		<div class="block">
			<%=Html.Hidden("rtId", Model.Caracterizacao.responsaveisTecnicos[5].id, new { @class="hdnRTExecucaoPlanoRecuperacaoId" })%>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[5].nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[5].proprioDeclarante, new { @class = "text rtNome txtRTExecucaoPlanoRecuperacaoNome", maxlength = "80" })) %>
			</div>
			<div class="coluna30">
				<label for="RTProfissao">Profissão</label>
				<%= Html.DropDownList("RT", Model.profissoesLst[5], ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[5].proprioDeclarante, new { @class = "text rtProfissao ddlRTExecucaoPlanoRecuperacaoProfissao" }))%>
			</div>
			<div class="coluna30">
				<label for="RTCREA">Registro CREA/ES</label>
				<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[5].registroCREA, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacao.responsaveisTecnicos[5].proprioDeclarante, new { @class = "text rtCREA txtRTExecucaoPlanoRecuperacaoCREA", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna100">
				<div class="coluna65">
					<label for="DiametroTubulacaoVazaoMin">Número da ART de execução do Plano de Recuperação de Área Degradada</label>
				</div> <br />
				<div class="coluna30">
					<%= Html.TextBox("RT", Model.Caracterizacao.responsaveisTecnicos[5].numeroART, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text rtNumero txtRTExecucaoPlanoRecuperacaoNumero", maxlength = "14" })) %>
				</div>
			</div>
		</div>
	</fieldset>

</fieldset>