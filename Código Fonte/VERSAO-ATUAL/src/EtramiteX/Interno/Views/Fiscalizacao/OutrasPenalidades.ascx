<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OutrasPenalidadesVM>" %>

<script>

	FiscalizacaoOutrasPenalidades.settings.urls.salvar = '<%= Url.Action("CriarOutrasPenalidades") %>';
	FiscalizacaoOutrasPenalidades.settings.urls.obterSerie = '<%= Url.Action("ObterInfracaoSeries") %>';
	FiscalizacaoOutrasPenalidades.settings.urls.enviarArquivo= '<%= Url.Action("Arquivo", "Arquivo") %>';
	FiscalizacaoOutrasPenalidades.settings.urls.obter = '<%= Url.Action("OutrasPenalidades") %>';
                
	FiscalizacaoOutrasPenalidades.container = $('.divContainer');
                
	FiscalizacaoOutrasPenalidades.settings.mensagens = <%= Model.Mensagens %>;
	FiscalizacaoOutrasPenalidades.TiposArquivo = <%= Model.TiposArquivoValido %>;

</script>

<div class="divContainer" >

    <input type="hidden" class="hdnOutrasPenalidadesId" value="<%:Model.OutrasPenalidades.Id %>" />

    <fieldset class="block box">
        <legend>Outras Penalidades</legend>

        <div class="block">
            <div class="coluna20">
                <label>IUF para Outras Penalidades *</label><br />
		        <label><%= Html.RadioButton("OutrasPenalidades.IsDigital", 0, (Model.OutrasPenalidades.IsDigital == null ? false : Model.OutrasPenalidades.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsDigital" }))%>Digital</label><br />
		        <label><%= Html.RadioButton("OutrasPenalidades.IsDigital", 1, (Model.OutrasPenalidades.IsDigital == null ? false : !Model.OutrasPenalidades.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsBloco" }))%>Bloco</label>
            </div>
        </div>
    </fieldset>

    <fieldset class="block box fsCamposOutrasPenalidades">
        <div class="block">
            <div class="coluna20">
		        <label>Número do IUF *</label>
		        <%= Html.TextBox("OutrasPenalidades.NumeroIUF", ((Model.OutrasPenalidades.IsDigital == true && Model.OutrasPenalidades.NumeroIUF == null) ? "Gerado automaticamente" : Model.OutrasPenalidades.NumeroIUF), ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.OutrasPenalidades.IsDigital == true), new { @class = "text maskNumInt txtNumeroIUF", @maxlength = "6" }))%>
	        </div>

            <div class="coluna17">
				<label>Série *</label><br />
				<%= Html.DropDownList("OutrasPenalidades.Serie", Model.Series, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Series.Count <= 2 || Model.OutrasPenalidades.IsDigital == true), new { @class = "text ddlSeries" }))%>
			</div>

            <div class="coluna15">
				<label>Data da lavratura do IUF *</label>
				<%= Html.TextBox("OutrasPenalidades.DataLavratura", (Model.OutrasPenalidades.DataLavratura.Data != DateTime.MinValue ? Model.OutrasPenalidades.DataLavratura.DataTexto : "Gerado automaticamente"), ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.OutrasPenalidades.IsDigital == true), new { @class = "text maskData txtDataLavratura" }))%>
			</div>
        </div>
        
        <br />
        
        <div class="block">
			<div class="coluna85">
				<label>Descrição de outras penalidades *</label>
				<%= Html.TextArea("OutrasPenalidades.Descricao", Model.OutrasPenalidades.Descricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtDescricao", @maxlength = "1000" }))%>
			</div>
		</div>

    </fieldset>

</div>