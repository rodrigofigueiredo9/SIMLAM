<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DominioVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script>
	Dominio.settings.mensagens = <%= Model.Mensagens %>;
	Dominio.settings.urls.reservaLegal = '<%= Url.Action("ReservaLegal", "Dominialidade") %>';
	Dominio.settings.urls.reservaLegalVisualizar = '<%= Url.Action("ReservaLegalVisualizar", "Dominialidade") %>';
	Dominio.settings.urls.validarSalvar = '<%= Url.Action("DominioValidarSalvar", "Dominialidade") %>';
	Dominio.idsTela = <%= Model.IdsTela %>;
</script>

<h1 class="titTela"><%: (Model.IsVisualizar) ? "Visualizar" : "Salvar" %> Dados do Domínio</h1>
<br />

<div class="block box">
    <input type="hidden" class="hdnDominioJson" value="<%: Json.Encode(new { 
        DominioId = Model.Dominio.Id, 
        EmpreendimentoLocalizacao = Model.Dominio.EmpreendimentoLocalizacao,
        AreaCroqui= Model.Dominio.AreaCroqui, 
        APPCroqui= Model.Dominio.APPCroqui 
    })%>" />

	<div class="block">
		<div class="coluna22 append4">
			<label for="">Identificação</label>
			<%= Html.TextBox("DominioIdentificacao", Model.Dominio.Identificacao, new { @class = "text txtIdentificacao disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna24">
			<p><label for="Dominio_Tipo">Tipo</label></p>
			<label><%= Html.RadioButton("DominioTipo", (int)eDominioTipo.Matricula, Model.Dominio.Tipo == eDominioTipo.Matricula, new { @class = "radio rdbTipo disabled", @disabled = "disabled" })%> Matrícula</label>
			<label class="append5"><%= Html.RadioButton("DominioTipo", (int)eDominioTipo.Posse, Model.Dominio.Tipo == eDominioTipo.Posse, new { @class = "radio rdbTipo disabled", @disabled = "disabled" })%> Posse</label>
		</div>
	</div>

	<% if(Model.Dominio.Tipo == eDominioTipo.Matricula) { %>
		<div class="block">
			<div class="coluna22 append2">
				<label for="Dominio_Matricula">Matrícula Nº *</label>
				<%= Html.TextBox("DominioMatricula", Model.Dominio.Matricula, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtMatricula setarFoco", @maxlength = "24" }))%>
			</div>

			<div class="coluna24 append2">
				<label for="Dominio_Folha">Folha Nº *</label>
				<%= Html.TextBox("DominioFolha", Model.Dominio.Folha, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFolha", @maxlength = "24" }))%>
			</div>

			<div class="coluna22">
				<label for="Dominio_Livro">Livro Nº *</label>
				<%= Html.TextBox("DominioLivro", Model.Dominio.Livro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtLivro", @maxlength = "24" }))%>
			</div>
		</div>

		<div class="block">
			<div class="ultima">
				<label for="Dominio_Cartorio">Cartório *</label>
				<%= Html.TextBox("DominioCartorio", Model.Dominio.Cartorio, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCartorio", @maxlength = "80" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna22 append2">
				<label for="Dominio_AreaCroqui">Área matrícula croqui (m²)</label>
				<%= Html.TextBox("DominioAreaCroqui", Model.Dominio.AreaCroquiTexto, new { @class = "text txtAreaCroqui disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna24 append2">
				<label for="Dominio_AreaDocumento">Área matrícula documento (m²) *</label>
				<%= Html.TextBox("DominioAreaDocumento", Model.Dominio.AreaDocumentoTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaDocumento maskDecimalPonto", @maxlength = "20" }))%>
			</div>

			<div class="coluna22">
				<label for="Dominio_ARLDocumento">Área RL documento (m²)</label>
				<%= Html.TextBox("DominioARLDocumento", Model.Dominio.ARLDocumentoTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtARLDocumento maskDecimalPonto", @maxlength = "20" }))%>
			</div>
		</div>
	<% } %>

	<% if(Model.Dominio.Tipo == eDominioTipo.Posse) { %>
		<div class="block">
			<div class="coluna22 append2">
				<label for="Dominio_ComprovacaoId">Comprovação *</label>
				<%= Html.DropDownList("DominioComprovacaoId", Model.Comprovacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlComprovacao setarFoco" }))%>
			</div>

			<div class="coluna24 append2">
				<label for="Dominio_AreaCroqui">Área posse croqui (m²)</label>
				<%= Html.TextBox("DominioAreaCroqui", Model.Dominio.AreaCroquiTexto, new { @class = "text txtAreaCroqui disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna22 append2">
				<label for="DominioAreaDocumento">Área posse documento (m²) <label class="asteriscoAreaMatDoc">*</label></label>
                <%= Html.TextBox("DominioAreaDocumento", Model.Dominio.AreaDocumentoTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaDocumento maskDecimalPonto", @maxlength = "20" }))%>
			</div>

			<div class="coluna22">
				<label for="Dominio_ARLDocumento">Área RL documento (m²)</label>
				<%= Html.TextBox("DominioARLDocumento", Model.Dominio.ARLDocumentoTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtARLDocumento maskDecimalPonto", @maxlength = "20" }))%>
			</div>
		</div>

		<div class="block divRegistro hide">
			<div class="ultima">
				<label for="Dominio_Registro">Descrição da comprovação <label class="asteriscoRegistro">*</label></label>
				<%= Html.TextBox("DominioRegistro", Model.Dominio.DescricaoComprovacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtRegistro", @maxlength = "80" }))%>
			</div>
		</div>
	<% } %>

	<div class="block">
		<div class="coluna22 append2">
			<label for="Dominio_APPCroqui">APP Croqui (m²)</label>
			<%= Html.TextBox("DominioAPPCroqui", Model.Dominio.APPCroquiTexto, new { @class = "text txtAPPCroqui disabled", @disabled = "disabled" })%>
		</div>

		<%if(Model.Dominio.EmpreendimentoLocalizacao == (int)eEmpreendimentoLocalizacao.ZonaRural){%>
		<div class="coluna24 append2">
			<label for="Dominio_NumeroCCIR">CCIR Nº</label>
			<%= Html.TextBox("DominioNumeroCCIR", Model.Dominio.NumeroCCIR, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtNumeroCCIR", @maxlength = "13" }))%>
		</div>

		<div class="coluna22 append2">
			<label for="Dominio_AreaCCIR">Área no CCIR (m²)</label>
			<%= Html.TextBox("DominioAreaCCIR", Model.Dominio.AreaCCIRTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCCIR maskDecimalPonto", @maxlength = "20" }))%>
		</div>

		<div class="coluna22">
			<label for="Dominio_DataUltimaAtualizacao_DataTexto">Data da ultima atualização</label>
			<%= Html.TextBox("DominioDataUltimaAtualizacaoDataTexto", Model.Dominio.DataUltimaAtualizacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataUltimaAtualizacao" }))%>
		</div>
		<%}%>
	</div>
</div>

<fieldset class="block box">
	<legend>Reserva Legal</legend>
	<%if (!Model.IsVisualizar){ %>
		<div class="ultima block">
			<button class="btnReservaAdicionar direita" title="Adicionar Reserva Legal">+ Reserva Legal</button>
		</div>
	<%} %>

	<div class="block dataGrid">
		<table class="dataGridTable tabReservasLegais" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th width="10%">Identificação</th>
					<th width="12%">Compensação</th>
					<th width="12%">Situação</th>
					<th>Termo/Matrícula compensada</th>
					<th width="14%">Situação vegetal</th>
					<th width="16%">Área RL (m²)</th>
					<% if(Model.IsVisualizar) { %>
					<th width="5%">Ação</th>
					<% } else { %>
					<th width="12%">Ações</th>
					<% } %>
				</tr>
			</thead>
			<tbody>
			<% string termoMatriculaCompensada = string.Empty; %>
			<% foreach (var reserva in Model.Dominio.ReservasLegais) { %>
			<% termoMatriculaCompensada = Model.ObterTermoMatriculaCompensada(reserva); %>
				<tr>
					<td><span class="identificacao" title="<%: reserva.Identificacao %>"><%: reserva.Identificacao%></span></td>
					<td><span class="Compensacao" title="<%=reserva.Compensacao %>"><%=reserva.Compensacao %></span></td>
                    <td><span class="situacao" title="<%: reserva.SituacaoTexto %>"><%: reserva.SituacaoTexto%></span></td>
					<td><span class="termoMatriculaCompensada" title="<%: termoMatriculaCompensada %>"><%: termoMatriculaCompensada %></span></td>
					<td><span class="situacaoVegetal" title="<%: reserva.SituacaoVegetalTexto %>"><%: reserva.SituacaoVegetalTexto%></span></td>
					<td><span class="areaRL" title="<%: reserva.AreaReservaLegal %>"><%: reserva.AreaReservaLegal%></span></td>

					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(reserva) %>' />
						<input title="Visualizar" type="button" class="icone visualizar btnReservaVisualizar" value="" />
						<% if(!Model.IsVisualizar) { %>
						<input title="Editar" type="button" class="icone editar btnReservaEditar" value="" />
						<% if(string.IsNullOrEmpty(reserva.Identificacao) || reserva.Excluir) { %><input title="Excluir" type="button" class="icone excluir btnReservaExcluir" value="" /><% } %>
						<% } %>
					</td>
				</tr>
			<% } %>
			<% if(!Model.IsVisualizar) { %>
				<tr class="trTemplateRow hide">
					<td><span class="identificacao"></span></td>
					<td><span class="compensacao"></span></td>
                    <td><span class="situacao"></span></td>
					<td><span class="termoMatriculaCompensada"></span></td>
					<td><span class="situacaoVegetal"></span></td>
					<td><span class="areaRL"></span></td>

					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="" />
						<input title="Visualizar" type="button" class="icone visualizar btnReservaVisualizar" value="" />
						<input title="Editar" type="button" class="icone editar btnReservaEditar" value="" />
						<input title="Excluir" type="button" class="icone excluir btnReservaExcluir" value="" />
					</td>
				</tr>
			<% } %>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="block box filtroExpansivoAberto">
	<legend class="titFiltros">Confrontações do domínio</legend>

	<div class="block filtroCorpo">
		<div class="block">
			<label for="Dominio_ConfrontacaoNorte">Norte *</label>
			<%= Html.TextArea("DominioConfrontacaoNorte", Model.Dominio.ConfrontacaoNorte, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacaoNorte setarFoco", @maxlength = "250" }))%>
		</div>

		<div class="block">
			<label for="Dominio_ConfrontacaoSul">Sul *</label>
			<%= Html.TextArea("DominioConfrontacaoSul", Model.Dominio.ConfrontacaoSul, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacaoSul", @maxlength = "250" }))%>
		</div>

		<div class="block">
			<label for="Dominio_ConfrontacaoLeste">Leste *</label>
			<%= Html.TextArea("DominioConfrontacaoLeste", Model.Dominio.ConfrontacaoLeste, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacaoLeste", @maxlength = "250" }))%>
		</div>

		<div class="block">
			<label for="Dominio_ConfrontacaoOeste">Oeste *</label>
			<%= Html.TextArea("DominioConfrontacaoOeste", Model.Dominio.ConfrontacaoOeste, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacaoOeste", @maxlength = "250" }))%>
		</div>
	</div>
</fieldset>