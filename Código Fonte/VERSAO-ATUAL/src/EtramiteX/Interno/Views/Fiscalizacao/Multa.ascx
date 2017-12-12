<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MultaVM>" %>

<script>

	FiscalizacaoMulta.settings.urls.salvar = '<%= Url.Action("CriarMulta") %>';
	FiscalizacaoMulta.settings.urls.obterSerie = '<%= Url.Action("ObterInfracaoSeries") %>';
	FiscalizacaoMulta.settings.urls.enviarArquivo= '<%= Url.Action("Arquivo", "Arquivo") %>';
	FiscalizacaoMulta.settings.urls.obter = '<%= Url.Action("Multa") %>';

	FiscalizacaoMulta.container = $('.divContainer');

	FiscalizacaoMulta.settings.mensagens = <%= Model.Mensagens %>;
	FiscalizacaoMulta.TiposArquivo = <%= Model.TiposArquivoValido %>;

</script>

<div class="divContainer" >

    <input type="hidden" class="hdnMultaId" value="<%:Model.Multa.Id %>" />

    <fieldset class="block box">
        <legend>Multa</legend>

        <div class="block">
            <div class="coluna20">
                <label>IUF para multa *</label><br />
		        <label><%= Html.RadioButton("Multa.IsDigital", 0, (Model.Multa.IsDigital == null ? false : Model.Multa.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsDigital" }))%>Digital</label><br />
		        <label><%= Html.RadioButton("Multa.IsDigital", 1, (Model.Multa.IsDigital == null ? false : !Model.Multa.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsBloco" }))%>Bloco</label>
            </div>
        </div>
    </fieldset>

    <fieldset class="block box fsCamposMulta">
        <div class="block">
            <div class="coluna20">
		        <label>Número do IUF *</label>
		        <%= Html.TextBox("Multa.NumeroIUF", ((Model.Multa.IsDigital == true && Model.Multa.NumeroIUF == null) ? "Gerado automaticamente" : Model.Multa.NumeroIUF), ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Multa.IsDigital == true), new { @class = "text maskNumInt txtNumeroIUF", @maxlength = "6" }))%>
	        </div>

            <div class="coluna17">
				<label>Série *</label><br />
				<%= Html.DropDownList("Multa.Serie", Model.Series, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Series.Count <= 2 || Model.Multa.IsDigital == true), new { @class = "text ddlSeries" }))%>
			</div>

            <div class="coluna20">
				<label>Data da lavratura do IUF *</label>
				<%= Html.TextBox("Multa.DataLavratura", (Model.Multa.DataLavratura.Data != DateTime.MinValue ? Model.Multa.DataLavratura.DataTexto : "Gerado automaticamente"), ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Multa.IsDigital == true), new { @class = "text maskData txtDataLavratura" }))%>
			</div>

            <div class="coluna17">
				<label>Código da receita *</label><br />
				<%= Html.DropDownList("Multa.CodigoReceita", Model.CodigosReceita, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.CodigosReceita.Count <= 2, new { @class = "text ddlCodigosReceita" }))%>
			</div>

            <div class="coluna20">
		        <label>Valor da multa (R$) *</label>
		        <%= Html.TextBox("Multa.ValorMulta", Model.Multa.ValorMultaMascara, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtValorMulta", @maxlength = "13" }))%>
	        </div>
        </div>
        
        <br />
        
        <div class="block">
			<div class="coluna85">
				<label>Justificar o valor da penalidade pecuniária atribuída, levando-se em consideração os parâmetros legais *</label>
				<%= Html.TextArea("Multa.Justificativa", Model.Multa.Justificativa, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtJustificativa", @maxlength = "1000" }))%>
			</div>
		</div>

    </fieldset>

</div>