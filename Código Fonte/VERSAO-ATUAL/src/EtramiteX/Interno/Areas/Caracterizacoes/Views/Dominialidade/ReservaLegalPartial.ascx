<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReservaLegalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>

<script>
	ReservaLegal.settings.idsTela = <%= Model.IdsTela %>;
	ReservaLegal.settings.urls.validarSalvar = '<%= Url.Action("ReservaLegalValidarSalvar", "Dominialidade") %>';
	ReservaLegal.settings.urls.obterDadosEmpreendimentoCompensacao = '<%= Url.Action("ObterDadosEmpreendimentoCompensacao", "Dominialidade") %>';
	ReservaLegal.settings.urls.empreendimentoAssociar = '<%= Url.Action("Associar", "Empreendimento") %>';
	ReservaLegal.settings.urls.empreendimentoAssociarCompensacao ='<%= Url.Action("AssociarCompensacao", "Empreendimento") %>'; 
	ReservaLegal.settings.urls.obterARLCompensacao = '<%= Url.Action("ObterCompensacaoARL", "Dominialidade") %>';
	ReservaLegal.settings.urls.coordenadaGeo = '<%= Url.Action("CoordenadaPartial", "Mapa", new {area="GeoProcessamento" })%>';
	ReservaLegal.settings.urls.obterARLCedente = '<%= Url.Action("ObterARLCedente", "Dominialidade")%>';
	
	ReservaLegal.settings.mensagens = <%= Model.Mensagens %>;


</script>

<h1 class="titTela"><%: (Model.IsVisualizar) ? "Visualizar" : "Salvar" %> Reserva Legal</h1>
<br />

<input type="hidden" class="hdnReservaLegalJson" value="<%: Json.Encode(new {
	ReservaLegalId =  Model.ReservaLegal.Id,
	ReservaLegalCompensada = Model.ReservaLegal.Compensada,
	ReservaLegalSituacaoVegetalId = Model.ReservaLegal.SituacaoVegetalId,
	MatriculaIdentificacao = Model.ReservaLegal.MatriculaIdentificacao,
	ARLCroqui = Model.ReservaLegal.ARLCroqui,
	Excluir = Model.ReservaLegal.Excluir
})%>" />

<div class="block box">
	<div class="block filtroCorpo">
		<div class="block">
			<div class="coluna18 append2">
				<label for="ReservaLegal_SituacaoId">Situação *</label>
				<%= Html.DropDownList("SituacaoId", Model.Situacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlSituacao setarFoco" }))%>
			</div>
			<div class="coluna45 <%= (Model.ReservaLegal.SituacaoId > 0 && Model.ReservaLegal.SituacaoId != (int)eReservaLegalSituacao.NaoInformada) ? "" : "hide" %>">
				<label for="ReservaLegal_LocalizacaoId">Localização *</label>
				<%= Html.DropDownList("LocalizacaoId_", Model.Localizacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlLocalizacao" }))%>
			</div>
		</div>

		<div class="block receptora <%=Model.ReservaLegal.CompensacaoTipo == eCompensacaoTipo.Receptora? "":"hide" %> ">
			<div class="coluna25">
				<label for="ReservaLegal_Cedente_Possui_Codigo_Empreendimento">O cedente possui código do empreendimento?*</label>
				<%= Html.DropDownList("CedentePossuiCodigoEmpreendimento", Model.ListaBooleana, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCedentePossuiCodigoEmpreendimento" }))%>
			</div>
		</div>

		<% if (!string.IsNullOrEmpty(Model.ReservaLegal.Identificacao)){ %>
		<div class="block">
			<div class="coluna18 append2">
				<label for="ReservaLegal_Identificacao">Identificação</label>
				<%= Html.TextBox("Identificacao", Model.ReservaLegal.Identificacao, new { @class = "text txtIdentificacao disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna21 append2">
				<label for="ReservaLegal_SituacaoVegetalTexto">Situação vegetal</label>
				<%= Html.TextBox("SituacaoVegetalTexto", Model.ReservaLegal.SituacaoVegetalTexto, new { @class = "text txtSituacaoVegetal disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna21">
				<label for="ReservaLegal_ARLCroqui">Área RL croqui (m²)</label>
				<%= Html.TextBox("ReservaLegal.ARLCroquiTexto", Model.ReservaLegal.ARLCroquiTexto, new { @class = "text txtARLCroqui disabled", @disabled = "disabled" })%>
			</div>
		</div>
		<% } %>

		<div class="divCamposSituacaoRegistrada <%=(Model.ReservaLegal.SituacaoId == (int)eReservaLegalSituacao.Registrada) ? "" : "hide" %>">

			<div class="block">
				<div class="coluna18 append2">
					<label for="ReservaLegal_NumeroTermo">Termo Nº</label>
					<%= Html.TextBox("NumeroTermo", Model.ReservaLegal.NumeroTermo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroTermo", @maxlength = "20" }))%>
				</div>

				<div class="coluna21 append2">
					<label for="ReservaLegal_NumeroTermo">Nº de Matrícula 
						<span class="receptora <%=Model.ReservaLegal.CompensacaoTipo == eCompensacaoTipo.Receptora? "":"hide" %>">do Cedente</span> *
					</label>
					<%= Html.TextBox("MatriculaNumero", Model.ReservaLegal.MatriculaNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtMatriculaNumero", @maxlength = "50" }))%>
				</div>

				<div class="coluna21 append2">
					<label for="ReservaLegal_TipoCartorioId">Tipo do cartório *</label>
					<%= Html.DropDownList("TipoCartorioId_", Model.Cartorios, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipoCartorio" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna75">
					<label for="ReservaLegal_NomeCartorio">Nome cartório</label>
					<%= Html.TextBox("NomeCartorio", Model.ReservaLegal.NomeCartorio, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNomeCartorio", @maxlength = "80" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna18 append2">
					<label for="ReservaLegal_NumeroFolha">Nº da Averbação*</label>
					<%= Html.TextBox("AverbacaoNumero", Model.ReservaLegal.AverbacaoNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAverbacaoNumero", @maxlength = "20" }))%>
				</div>

				<div class="coluna21 append2">
					<label for="ReservaLegal_NumeroLivro">Nº do livro</label>
					<%= Html.TextBox("NumeroLivro", Model.ReservaLegal.NumeroLivro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroLivro", @maxlength = "7" }))%>
				</div>

				<div class="coluna21">
					<label for="ReservaLegal_NumeroFolha">Nº da folha</label>
					<%= Html.TextBox("NumeroFolha", Model.ReservaLegal.NumeroFolha, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroFolha", @maxlength = "7" }))%>
				</div>

			</div>

			<div class="block  receptora <%=Model.ReservaLegal.CompensacaoTipo == eCompensacaoTipo.Receptora && Model.ReservaLegal.CedentePossuiEmpreendimento == 0 ? "": "hide" %>">
				<div class="coluna18 append2">
					<label for="ARL_Cedida">Área da RL cedida (m²)*</label>
					<%= Html.TextBox("ARLCedida", Model.ReservaLegal.ARLCedida > 0? Model.ReservaLegal.ARLCedida.ToString() : "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "maskDecimalPonto text txtARLCedida", @maxlength = "20"}))%>
				</div>

				<div class="coluna21">
					<label for="Situacao_Vegetal_RL">Situação vegetal da RL cedida*</label>
					<%= Html.DropDownList("ReservaLegal.SituacaoVegetalId", Model.SituacoesVegetal, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlSituacaoVegetal"}))%>
				</div>
			</div>

			<div class="block">

				<div class="coluna18 cedente append2 <%=Model.ReservaLegal.CompensacaoTipo == eCompensacaoTipo.Cedente? "": "hide" %>">
					<label for="ARL_Recebida">Área da RL recebida (m²)*</label>
					<%= Html.TextBox("ARLRecebida", Model.ReservaLegal.ARLRecebida > 0? Model.ReservaLegal.ARLRecebida.ToString() : "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "maskDecimalPonto text txtARLRecebida", @maxlength = "20" }))%>
				</div>

				<div class="coluna18 coordenada <%=Model.ReservaLegal.CedentePossuiEmpreendimento == 1? "hide": "" %>">
					<label for="Coordenada_Tipo_Id">Sistema de coordenada</label>
					<%= Html.DropDownList("ReservaLegal.Coordenada.Tipo.Id", Model.TiposCoordenada, new { @class = "text disabled ddlCoordenadaTipo", @disabled = "disabled" })%>
				</div>

				<div class="coluna21 prepend2 coordenada <%=Model.ReservaLegal.CedentePossuiEmpreendimento == 1? "hide": "" %>">
					<label for="Coordenada_Datum_Id">Datum</label>
					<%= Html.DropDownList("ReservaLegal.Coordenada.Datum.Id", Model.Datuns, new { @class = "text disabled ddlDatum", @disabled = "disabled" })%>
				</div>

			</div>

			<div class="block">
				<div class="coluna18 coordenada <%=Model.ReservaLegal.CedentePossuiEmpreendimento == 1? "hide": "" %>">
					<label for="Coordenada_EastingUtmTexto">Easting / Longitude</label>
					<%= Html.TextBox("ReservaLegal.Coordenada.EastingUtmTexto", Model.ReservaLegal.Coordenada.EastingUtm, new { @class = "text disabled txtEasting", @disabled = "disabled" })%>
				</div>

				<div class="coluna21 prepend2 coordenada <%=Model.ReservaLegal.CedentePossuiEmpreendimento == 1? "hide": "" %>">
					<label for="Filtros_Coordenada_NorthingUtmTexto">Northing / Latitude</label>
					<%= Html.TextBox("ReservaLegal.Coordenada.NorthingUtmTexto", Model.ReservaLegal.Coordenada.NorthingUtm, new { @class = "text disabled txtNorthing", @disabled = "disabled" })%>
				</div>

				<%if (!Model.IsVisualizar){ %>
					<div class="coluna20 receptora <%= (Model.ReservaLegal.SituacaoId == (int)eReservaLegalSituacao.NaoInformada || Model.ReservaLegal.CompensacaoTipo == eCompensacaoTipo.Receptora) && Model.ReservaLegal.CedentePossuiEmpreendimento == 0 ? "": "hide" %>">
						<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
					</div>
				<%} %>

			</div>

		</div>

		<fieldset class="block boxBranca divCompensacao 
			<%= (Model.ReservaLegal.CompensacaoTipo == eCompensacaoTipo.Receptora && Model.ReservaLegal.CedentePossuiEmpreendimento.GetValueOrDefault() == 1)  ||
			Model.ReservaLegal.CompensacaoTipo == eCompensacaoTipo.Cedente? "":"hide" %>">
			
			
			<legend>Dados do <%=Model.ReservaLegal.Compensada? "Receptor" : "Cedente" %></legend>

			<div class="block">
				<div class="coluna15">
					<label for="Codigo_Empreendimento">Código do empreendimento</label>
					<%= Html.TextBox("ReservaLegal.EmpreendimentoCompensacao.Codigo", Model.ReservaLegal.EmpreendimentoCompensacao.Codigo, new { @class = "text disabled txtEmpreendimentoCompCodigo", @disabled = "disabled" })%>
				</div>

				<div class="coluna30 prepend2">
					<label for="Razao_Social">Razão Social/ Denominação/ Nome</label>
					<%= Html.TextBox("ReservaLegal.EmpreendimentoCompensacao.Denominador", Model.ReservaLegal.EmpreendimentoCompensacao.Denominador, new { @class = "text disabled txtEmpreendimentoCompDenominador", @disabled = "disabled" })%>
				</div>

				<div class="coluna15 prepend2">
					<label for="CNPJ">CNPJ</label>
					<%= Html.TextBox("ReservaLegal.EmpreendimentoCompensacao.CNPJ", Model.ReservaLegal.EmpreendimentoCompensacao.CNPJ, new { @class = "text disabled txtEmpreendimentoCompCNPJ", @disabled = "disabled" })%>
				</div>
				<%if(!Model.IsVisualizar){ %>
				<div class="coluna10 prepend2">
					<button type="button" class="inlineBotao btnBuscarEmpreendimento">Buscar</button>
				</div>
				<%} %>
				<%=Html.Hidden("ReservaLegal.EmpreendimentoCompensacao", Model.ReservaLegal.EmpreendimentoCompensacao.Id > 0 ? Model.ReservaLegal.EmpreendimentoCompensacao.Id.ToString(): "", new { @class="hdnEmpreendimentoCompensacaoId"})%>
			</div>

			<div class="block">
				<div class="coluna60">
					<label for="ReservaLegal_MatriculaIdentificacao">Matrícula/ Posse*</label>
					<%= Html.DropDownList("MatriculaIdentificacao_", Model.MatriculaCompensacao, ViewModelHelper.SetaDisabled(Model.MatriculaCompensacao.Count <= 2 || Model.IsVisualizar, new { @class = "text ddlMatriculasCompensacao"}))%>
				</div>

			</div>

			<div class="block divIdentificacaoARL <%=Model.ReservaLegal.CompensacaoTipo == eCompensacaoTipo.Receptora? "":"hide" %>">
				<label for="ARL_Cedente">Identificação da ARL*</label>
				<%= Html.DropDownList("ARL_Identificacao", Model.IdentificacaoARLCompensacao, ViewModelHelper.SetaDisabled(Model.IdentificacaoARLCompensacao.Count <= 2 || Model.IsVisualizar, new { @class = "text ddlARLIdentificacaoCompensacao"}))%>
				<%=Html.Hidden("Dados_Cedente", Model.ReservaCedenteJson, new  {@class="hdnDadosCedente" })%>
			</div>

		</fieldset>
	</div>
</div>
