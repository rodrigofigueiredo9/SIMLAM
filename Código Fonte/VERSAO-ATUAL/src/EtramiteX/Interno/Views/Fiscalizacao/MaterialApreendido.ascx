<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaterialApreendidoVM>" %>

<script>

	FiscalizacaoMaterialApreendido.settings.urls.salvar = '<%= Url.Action("CriarMaterialApreendido") %>';
	FiscalizacaoMaterialApreendido.settings.urls.obterSerie = '<%= Url.Action("ObterInfracaoSeries") %>';
	FiscalizacaoMaterialApreendido.settings.urls.associarDepositario = '<%= Url.Action("PessoaModal", "Pessoa") %>';
	FiscalizacaoMaterialApreendido.settings.urls.editarDepositario = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';
	FiscalizacaoMaterialApreendido.settings.urls.enviarArquivo= '<%= Url.Action("Arquivo", "Arquivo") %>';
	FiscalizacaoMaterialApreendido.settings.urls.obter = '<%= Url.Action("MaterialApreendido") %>';

	FiscalizacaoMaterialApreendido.container = $('.divContainer');

	FiscalizacaoMaterialApreendido.settings.mensagens = <%= Model.Mensagens %>;
	FiscalizacaoMaterialApreendido.TiposArquivo = <%= Model.TiposArquivoValido %>;

</script>

<div class="divContainer" >

    <input type="hidden" class="hdnMaterialApreendidoId" value="<%:Model.MaterialApreendido.Id %>" />

    <fieldset class="block box">
        <legend>Apreensão</legend>

        <div class="block">
            <div class="coluna20">
                <label>IUF para Apreensão</label><br />
		        <label><%= Html.RadioButton("MaterialApreendido.IsDigital", 1, (Model.MaterialApreendido.IsDigital == null ? false : Model.MaterialApreendido.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsDigital" }))%>Digital</label><br />
		        <label><%= Html.RadioButton("MaterialApreendido.IsDigital", 0, (Model.MaterialApreendido.IsDigital == null ? false : !Model.MaterialApreendido.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsBloco" }))%>Bloco</label>
            </div>
        </div>

        <div class="block">
            <div class="coluna15">
		        <label>Número do IUF</label>
		        <%= Html.TextBox("MaterialApreendido.NumeroTad", Model.MaterialApreendido.NumeroTad, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtNumeroTad", @maxlength = "8" }))%>
	        </div>

            <div class="coluna15">
				<label>Série</label><br />
				<%= Html.DropDownList("MaterialApreendido.Serie", Model.Series, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Series.Count <= 2, new { @class = "text ddlSeries" }))%>
			</div>

            <div class="coluna15">
				<label>Data da lavratura do IUF</label>
				<%= Html.TextBox("MaterialApreendido.DataLavratura", Model.DataConclusaoFiscalizacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataLavratura" }))%>
			</div>
        </div>

        <div class="block">
            <div class="coluna50 inputFileDiv">
				<label>PDF do IUF</label>
				<div class="block">
					<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.MaterialApreendido.Arquivo.Id }) %>" class="<%= string.IsNullOrEmpty(Model.MaterialApreendido.Arquivo.Nome) ? "hide" : "" %> txtArquivoNome"><%= Html.Encode(Model.MaterialApreendido.Arquivo.Nome)%></a>
				</div>
				<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
				<span class="spanInputFile <%= string.IsNullOrEmpty(Model.MaterialApreendido.Arquivo.Nome) ? "" : "hide" %>">
					<input type="file" id="file" class="inputFile" style="display: block; width: 100%" name="file" <%=Model.IsVisualizar ? "disabled=\"disabled\"" : "" %>/>
				</span>
			</div>
			<% if (!Model.IsVisualizar) { %>
			    <div style="margin-top:8px" class="coluna40 prepend1 spanBotoes">
				    <button type="button" class="inlineBotao btnAddArq <%= string.IsNullOrEmpty(Model.MaterialApreendido.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
				    <button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.MaterialApreendido.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
			    </div>
			<% } %>
        </div>

        <div class="block">
			<div class="coluna75">
				<label>Descrever a apreensão *</label>
				<%= Html.TextArea("MaterialApreendido.Descricao", Model.MaterialApreendido.Descricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtDescricao", @maxlength = "250" }))%>
			</div>
		</div>

        <div class="block">
			<div class="coluna75">
				<label>Valor dos bens apreendidos (R$ e por extenso)</label>
				<%= Html.TextBox("MaterialApreendido.ValorProdutos", Model.MaterialApreendido.ValorProdutos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimal txtValorProdutos", @maxlength = "15" }))%>
			</div>
		</div>

        <div class="block">
			<div class="coluna75">
				<label>Número(s) do(s) Lacre(s) da Interdição/Embargo</label>
				<%= Html.TextBox("MaterialApreendido.ValorProdutos", Model.MaterialApreendido.ValorProdutos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimal txtValorProdutos", @maxlength = "15" }))%>
			</div>
		</div>

    </fieldset>

</div>