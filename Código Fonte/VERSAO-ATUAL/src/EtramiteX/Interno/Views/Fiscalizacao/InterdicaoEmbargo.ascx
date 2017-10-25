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
                <label>IUF para interdição/embargo *</label><br />
		        <label><%= Html.RadioButton("ObjetoInfracao.IsDigital", 0, (Model.Entidade.IsDigital == null ? false : Model.Entidade.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsDigital" }))%>Digital</label><br />
		        <label><%= Html.RadioButton("ObjetoInfracao.IsDigital", 1, (Model.Entidade.IsDigital == null ? false : !Model.Entidade.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsBloco" }))%>Bloco</label>
            </div>
        </div>
    </fieldset>

    <fieldset class="block box fsCamposInterdicaoEmbargo">
        <div class="block">
            <div class="coluna20">
		        <label>Número do IUF *</label>
		        <%= Html.TextBox("ObjetoInfracao.NumeroIUF", ((Model.Entidade.IsDigital == true && Model.Entidade.NumeroIUF == null) ? "Gerado automaticamente" : Model.Entidade.NumeroIUF), ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Entidade.IsDigital == true), new { @class = "text maskNumInt txtNumeroIUF", @maxlength = "8" }))%>
	        </div>

            <div class="coluna17">
				<label>Série *</label><br />
				<%= Html.DropDownList("ObjetoInfracao.Serie", Model.Series, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Series.Count <= 2 || Model.Entidade.IsDigital == true), new { @class = "text ddlSeries" }))%>
			</div>

            <div class="coluna20">
				<label>Data da lavratura do IUF *</label>
				<%= Html.TextBox("ObjetoInfracao.DataLavratura", (Model.Entidade.DataLavraturaTermo.Data != DateTime.MinValue ? Model.Entidade.DataLavraturaTermo.DataTexto : "Gerado automaticamente"), ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Entidade.IsDigital == true), new { @class = "text maskData txtDataLavratura" }))%>
			</div>
        </div>

        <div class="block divPDF">
            <div class="coluna50 inputFileDiv">
				<label>PDF do IUF</label>
				<div class="block">
					<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.Entidade.Arquivo.Id }) %>" class="<%= string.IsNullOrEmpty(Model.Entidade.Arquivo.Nome) ? "hide" : "" %> txtArquivoNome"><%= Html.Encode(Model.Entidade.Arquivo.Nome)%></a>
				</div>
				<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
				<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Entidade.Arquivo.Nome) ? "" : "hide" %>">
					<input type="file" id="file" class="inputFile" style="display: block; width: 100%" name="file" <%=Model.IsVisualizar ? "disabled=\"disabled\"" : "" %>/>
				</span>
			</div>
			<% if (!Model.IsVisualizar) { %>
			    <div style="margin-top:8px" class="coluna40 prepend1 spanBotoes">
				    <button type="button" class="inlineBotao btnAddArq <%= string.IsNullOrEmpty(Model.Entidade.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
				    <button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.Entidade.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
			    </div>
			<% } %>
        </div>
    </fieldset>

    <fieldset class="block box fsCamposInterdicaoEmbargo">
        <div class="block">
		    <div class="coluna10">
			    <label><%= Html.RadioButton("ObjetoInfracao.Interditado", 1, (Model.Entidade.Interditado == null ? false : Model.Entidade.Interditado.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbInterditado" }))%>Interditado</label>
		    </div>
            <div class="coluna10">
			    <label><%= Html.RadioButton("ObjetoInfracao.Interditado", 0, (Model.Entidade.Interditado == null ? false : !Model.Entidade.Interditado.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbEmbargado" }))%>Embargado</label>
		    </div>
        </div>

        <div class="block">
			<div style="margin-top:7px" class="coluna75">
				<label for="ObjetoInfracao_DescricaoTermoEmbargo">Descrição da interdição/embargo *</label>
				<%= Html.TextBox("ObjetoInfracao.DescricaoTermoEmbargo", Model.Entidade.DescricaoTermoEmbargo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDescricaoTermoEmbargo", @maxlength = "150" }))%>
			</div>
		</div>
        
        <div class="block">
			<div class="coluna75">
				<label for="ObjetoInfracao_OpniaoAreaDanificada">Opinar quanto à interdição/embargo da área/atividade/produto, justificando sua manutenção ou a possibilidade de desinterdição/desembargo</label>
				<%= Html.TextArea("ObjetoInfracao.OpniaoAreaDanificada", Model.Entidade.OpniaoAreaDanificada, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtOpniaoAreaDanificada", @maxlength = "1000" }))%>
			</div>
		</div>

        <div class="block">
			<div class="coluna75">
				<label>Número(s) do(s) Lacre(s) da interdição/embargo</label>
				<%= Html.TextBox("ObjetoInfracao.NumeroLacre", Model.Entidade.NumeroLacre, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroLacre", @maxlength = "100" }))%>
			</div>
		</div>
        
        <div class="block">
			<div class="coluna52">
				<label for="ObjetoInfracao_ExisteAtvAreaDegrad">Está sendo desenvolvida alguma atividade na área interditada/embargada? *</label><br />
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