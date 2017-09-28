<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ObjetoInfracaoVM>" %>

<div class="FiscalizacaoObjetoInfracaoContainer">
	<input type="hidden" class="hdnObjetoInfracaoId" value="<%:Model.Entidade.Id %>" />
	<input type="hidden" class="hdnIsVisualizar" value="<%:Model.IsVisualizar.ToString().ToLower()%>" />
	
    <fieldset class="block box">
        <legend>Interdição/Embargo</legend>

        <div class="block">
            <div class="coluna20">
                <label>IUF para Multa</label><br />
		        <label><%= Html.RadioButton("Multa.IsDigital", 0, (Model.Entidade.TeiGeradoPeloSistema == null ? false : true), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsDigital" }))%>Digital</label><br />
		        <label><%= Html.RadioButton("Multa.IsDigital", 1, (Model.Entidade.TeiGeradoPeloSistema == null ? false : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsBloco" }))%>Bloco</label>
            </div>
        </div>
    </fieldset>

    <fieldset class="block box fsCamposInterdicaoEmbargo">
        <div class="block">
            <div class="coluna20">
		        <label>Número do IUF</label>
		        <%= Html.TextBox("Multa.NumeroIUF", 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtNumeroIUF", @maxlength = "8" }))%>
	        </div>

            <div class="coluna17">
				<label>Série</label><br />
				<%= Html.DropDownList("Multa.Serie", Model.Series, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Series.Count <= 2, new { @class = "text ddlSeries" }))%>
			</div>

            <div class="coluna15">
				<label>Data da lavratura do IUF</label>
				<%= Html.TextBox("Multa.DataLavratura", Model.Entidade.DataLavraturaTermo.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataLavratura" }))%>
			</div>
        </div>
    </fieldset>

    <fieldset class="block box">
        <div class="block">
		    <div class="coluna10">
			    <label><%= Html.RadioButton("ObjetoInfracao.ExisteAtvAreaDegrad", 1, (Model.Entidade.ExisteAtvAreaDegrad == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteAtvAreaDegrad" }))%>Interditado</label>
		    </div>
            <div class="coluna10">
			    <label><%= Html.RadioButton("ObjetoInfracao.ExisteAtvAreaDegrad", 0, (Model.Entidade.ExisteAtvAreaDegrad == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteAtvAreaDegrad" }))%>Embargado</label>
		    </div>
        </div>

        <div class="block">
			<div style="margin-top:7px" class="coluna75">
				<label for="ObjetoInfracao_DescricaoTermoEmbargo">Descrição de embargo/interdição *</label>
				<%= Html.TextBox("ObjetoInfracao.DescricaoTermoEmbargo", Model.Entidade.DescricaoTermoEmbargo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDescricaoTermoEmbargo", @maxlength = "150" }))%>
			</div>
		</div>
        
        <div class="block">
			<div class="coluna75">
				<label for="ObjetoInfracao_OpniaoAreaDanificada">Opinar quanto ao embargo/interdição da área/atividade/produto, justificando sua manutenção ou a possibilidade de desembargo/desinterdição</label>
				<%= Html.TextArea("ObjetoInfracao.OpniaoAreaDanificada", Model.Entidade.OpniaoAreaDanificada, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtOpniaoAreaDanificada", @maxlength = "1000" }))%>
			</div>
		</div>

        <div class="block">
			<div class="coluna75">
				<label>Número(s) do(s) Lacre(s) da Interdição/Embargo</label>
				<%= Html.TextBox("MaterialApreendido.NumeroLacre", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroLacre", @maxlength = "100" }))%>
			</div>
		</div>
        
        <div class="block">
			<div class="coluna52">
				<label for="ObjetoInfracao_ExisteAtvAreaDegrad">Está sendo desenvolvida alguma atividade na área embargada/interditada? *</label><br />
				<span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpanExisteAtvAreaDegrad">
					<label><%= Html.RadioButton("ObjetoInfracao.ExisteAtvAreaDegrad", 1, (Model.Entidade.ExisteAtvAreaDegrad == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteAtvAreaDegrad" }))%>Sim</label>
					<label><%= Html.RadioButton("ObjetoInfracao.ExisteAtvAreaDegrad", 0, (Model.Entidade.ExisteAtvAreaDegrad == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteAtvAreaDegrad" }))%>Não</label>
					<label><%= Html.RadioButton("ObjetoInfracao.ExisteAtvAreaDegrad", -1, (Model.Entidade.ExisteAtvAreaDegrad == -1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteAtvAreaDegrad" }))%>Não se aplica</label>
				</span>
			</div>
		</div>
        
        <div class="block divExisteAtvAreaDegradEspecificarTexto">
			<div class="coluna75">
				<label for="ObjetoInfracao_ExisteAtvAreaDegradEspecificarTexto">Especificar *</label>
				<%= Html.TextArea("ObjetoInfracao.ExisteAtvAreaDegradEspecificarTexto", Model.Entidade.ExisteAtvAreaDegradEspecificarTexto, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtExisteAtvAreaDegradEspecificarTexto", @maxlength = "1000" }))%>
			</div>
		</div>
	</fieldset>
</div>