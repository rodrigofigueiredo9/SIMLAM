<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InfracaoVM>" %>

<script>

	Infracao.settings.urls.obterTipo = '<%= Url.Action("ObterInfracaoTipos") %>';
	Infracao.settings.urls.obterItem = '<%= Url.Action("ObterInfracaoItens") %>';
	Infracao.settings.urls.obterConfiguracao = '<%= Url.Action("ObterConfiguracao") %>';
	Infracao.settings.urls.obterSerie = '<%= Url.Action("ObterInfracaoSeries") %>';
	Infracao.settings.urls.salvar = '<%= Url.Action("CriarInfracao") %>';
	Infracao.settings.urls.enviarArquivo= '<%= Url.Action("Arquivo", "Arquivo") %>';
	Infracao.settings.urls.obter = '<%= Url.Action("Infracao") %>';
	Infracao.settings.mensagens = <%= Model.Mensagens %>;
	Infracao.container = $('.divContainer');
	Infracao.TiposArquivo = <%= Model.TiposArquivoValido %>;

</script>


<div class="divContainer" >

<input type="hidden" class="hdnInfracaoId" value="<%:Model.Infracao.Id %>" />
<input type="hidden" class="hdnConfigAlterou" value='<%:Model.Infracao.ConfigAlterou.ToString().ToLower() %>' />

<fieldset class="box">
	<legend>Classificação da infração</legend>

	<div class="block">
		<div class="coluna76">
			<label>Classificação *</label><br />
			<%= Html.DropDownList("Infracao.Classificacao", Model.Classificacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Classificacoes.Count <= 2, new { @class = "text ddlClassificacoes" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna76">
			<label>Tipo de infração *</label><br />
			<%= Html.DropDownList("Infracao.Tipo", Model.Tipos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipos" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna76">
			<label>Item *</label><br />
			<%= Html.DropDownList("Infracao.Item", Model.Itens, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlItens" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna76">
			<label>Subitem</label><br />
			<%= Html.DropDownList("Infracao.Subitem", Model.Subitens, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlSubitens" }))%>
		</div>
	</div>

	<div class="divCamposPerguntas" >
		<% Html.RenderPartial("InfracaoCamposPerguntas", Model); %>
	</div>

</fieldset>

<fieldset class="box">
	<legend>Dados do auto de infração</legend>
	<div class="block">
		<div class="coluna40">
			<label>Auto de infração? *</label><br />
			<label><%= Html.RadioButton("Infracao.IsAutuada", 1, (Model.Infracao.IsAutuada == null ? false : Model.Infracao.IsAutuada.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsAutuadaSim" }))%>Sim</label>
			<label class="append5"><%= Html.RadioButton("Infracao.IsAutuada", 0, (Model.Infracao.IsAutuada == null ? false : !Model.Infracao.IsAutuada.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsAutuadaNao" }))%>Não</label>
		</div>
	</div>

	<div class="divInfracaoAutuada <%= (Model.Infracao.IsAutuada == null || !Model.Infracao.IsAutuada.Value ? "hide" : "") %>" >

		<div class="block">
			<div class="coluna22 append2">
				<label>Gerar auto de infração? *</label><br />
				<label><%= Html.RadioButton("Infracao.IsGeradaSistema", 1, (Model.Infracao.IsGeradaSistema == null ? false : Model.Infracao.IsGeradaSistema.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsGeradaSistemaSim rdbIsGeradaSistema" }))%>Sim</label>
				<label class="append5"><%= Html.RadioButton("Infracao.IsGeradaSistema", 0, (Model.Infracao.IsGeradaSistema == null ? false : !Model.Infracao.IsGeradaSistema.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsGeradaSistemaNao rdbIsGeradaSistema" }))%>Não</label>
			</div>
			<div class="coluna15 append2">
				<label>Série *</label><br />
				<%= Html.DropDownList("Infracao.Serie", Model.Series, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlSeries" }))%>
			</div>

			<%if ((Model.Infracao.FiscalizacaoSituacaoId != (int)eFiscalizacaoSituacao.EmAndamento) && Model.Infracao.IsGeradaSistema.GetValueOrDefault()){%>
				<div class="coluna22 append2">
					<label>Data da lavratura do auto *</label>
					<%= Html.TextBox("Infracao.DataLavraturaAuto", Model.DataConclusaoFiscalizacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataLavraturaAuto maskData" }))%>
				</div>
			<%}else{ %>
				<div class="coluna22 append2 divIsGeradoSistema <%= (Model.Infracao.IsGeradaSistema == null || Model.Infracao.IsGeradaSistema.Value ? "hide" : "") %>">
					<label>Data da lavratura do auto *</label>
					<%= Html.TextBox("Infracao.DataLavraturaAuto", Model.Infracao.DataLavraturaAuto.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataLavraturaAuto maskData" }))%>
				</div>
			<%} %>


			<div class="coluna25 divIsGeradoSistema <%= (Model.Infracao.IsGeradaSistema == null || Model.Infracao.IsGeradaSistema.Value ? "hide" : "") %>">
				<label class="lblNumAutoInfracao">Nº do auto de infração - bloco *</label>
				<%= Html.TextBox("Infracao.NumeroAutoInfracaoBloco", Model.Infracao.NumeroAutoInfracaoBloco, ViewModelHelper.SetaDisabled(Model.IsVisualizar ,new { @class = "text maskNumInt txtNumeroAutoInfracaoBloco", @maxlength = "10" }))%>
			</div>
		</div>

		<div class="divIsGeradoSistema <%= (Model.Infracao.IsGeradaSistema == null || Model.Infracao.IsGeradaSistema.Value ? "hide" : "") %>">
			<div class="block">
				<div class="coluna35 inputFileDiv">
					<label>PDF do auto de infração</label>
					<div class="block">
						<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.Infracao.Arquivo.Id }) %>" class="<%= string.IsNullOrEmpty(Model.Infracao.Arquivo.Nome) ? "hide" : "" %> txtArquivoNome"><%= Html.Encode(Model.Infracao.Arquivo.Nome)%></a>
					</div>
					<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
					<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Infracao.Arquivo.Nome) ? "" : "hide" %>">
						<input type="file" id="file" class="inputFile" style="display: block" name="file" <%=Model.IsVisualizar ? "disabled=\"disabled\"" : "" %> />
					</span>
				</div>
				<% if (!Model.IsVisualizar) { %>
				<div style="margin-top:8px" class="coluna25 prepend1 spanBotoes">
					<button type="button" class="inlineBotao botaoAdicionar btnAddArq <%= string.IsNullOrEmpty(Model.Infracao.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
					<button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.Infracao.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
				</div>
				<% } %>
			</div>
		</div>

		<div class="block">
			<div class="coluna76">
				<label>Descrição da infração *</label>
				<%= Html.TextArea("Infracao.DescricaoInfracao", Model.Infracao.DescricaoInfracao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtDescricaoInfracao", @maxlength = "1000" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna18 append2">
				<label>Código da receita *</label>
				<%= Html.DropDownList("Infracao.CodigoReceita", Model.CodigoReceitas, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.CodigoReceitas.Count <= 2, new { @class = "text ddlCodigoReceitas" }))%>
			</div>
			<div class="coluna22">
				<label>Valor da multa (Reais) *</label>
				<%= Html.TextBox("Infracao.ValorMulta", Model.Infracao.ValorMulta,  ViewModelHelper.SetaDisabled(Model.IsVisualizar ,new { @class = "text maskDecimal txtValorMulta", @maxlength = "15" }))%>
			</div>
		</div>

	</div>

</fieldset>

</div>