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
        <legend>Tipo de Infração/Fiscalização</legend>
        <div class="block">
		    <div class="coluna18">
			    <label><%= Html.RadioButton("Infracao.ComInfracao", 1, (Model.Infracao.ComInfracao == null ? false : Model.Infracao.ComInfracao.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoComInfracao" }))%>Fiscalização com infração</label>
            </div>
            <div class="coluna18">
			    <label class="append5"><%= Html.RadioButton("Infracao.ComInfracao", 0, (Model.Infracao.ComInfracao == null ? false : !Model.Infracao.ComInfracao.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoSemInfracao" }))%>Fiscalização sem infração</label>
		    </div>
	    </div>
    </fieldset>

    <fieldset class="box fsInfracao fsCaracterizacao">
	    <legend>Caracterização da infração</legend>

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

    <fieldset class="box fsInfracao fsEnquadramento">
        <legend>Enquadramento da infração</legend>

        <div class="block dataGrid divQuadroEnquadramento">
			    <div class="coluna90">
				    <table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					    <thead>
						    <tr>
							    <th style="text-align:center; width:15%">Artigo</th>
                                <th style="text-align: center; width: 15%">Item/Parágrafo/Alínea</th>
                                <th style="text-align:center">Lei/Decreto/Resolução/Portaria/Instrução Normativa</th>
						    </tr>
					    </thead>
					    <tbody>
					        <% for (int linha = 0; linha < 3; linha++) { %>
						        <tr>
                                    <td>
                                        <input type="hidden" class="enquadramentoId" value="" />
                                        <input type="text" class="text txtArtigoEnquadramento" maxlength="16" style="border: none; width:100%; background-color:transparent; padding-top:10px;" />
                                    </td>
                                    <td>
                                        <input type="text" class="text txtItemEnquadramento" maxlength="16" style="border: none; width:100%; background-color:transparent; padding-top:10px;" />
                                    </td>
                                    <td>
                                        <input type="text" class="text txtLeiEnquadramento" maxlength="90" style="border: none; width:100%; background-color:transparent; padding-top:10px;" />
                                    </td>
						        </tr>
						    <% } %>
					    </tbody>
				    </table>
			    </div>
		    </div>
    </fieldset>

    <fieldset class="box fsInfracao fsDadosInfracao">
        <div class="block divDescricaoInfracao">
		    <div class="coluna76">
			    <label>Descrição da infração/fiscalização *</label>
			    <%= Html.TextArea("Infracao.DescricaoInfracao", Model.Infracao.DescricaoInfracao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtDescricaoInfracao", @maxlength = "1000" }))%>
		    </div>
	    </div>

        <div class="block">
            <div class="coluna20 append2">
			    <label>Data da constatação/vistoria *</label>
			    <%= Html.TextBox("Infracao.DataLavraturaAuto", Model.DataConclusaoFiscalizacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataLavraturaAuto maskData" }))%>
		    </div>

            <div class="coluna15 append2">
			    <label>Hora da constatação *</label>
			    <%= Html.TextBox("Infracao.DataLavraturaAuto", Model.DataConclusaoFiscalizacao.DataHoraTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskHoraMinuto txtDataLavraturaAuto" }))%>
		    </div>

            <div class="coluna40 divClassificacao">
                <label>Classificação da infração *</label>
                <br />
                <label><%= Html.RadioButton("Infracao.IsAutuada", 1, (Model.Infracao.IsAutuada == null ? false : Model.Infracao.IsAutuada.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsAutuadaSim" }))%>Leve</label>
			    <label><%= Html.RadioButton("Infracao.IsAutuada", 0, (Model.Infracao.IsAutuada == null ? false : !Model.Infracao.IsAutuada.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsAutuadaNao" }))%>Média</label>
                <label><%= Html.RadioButton("Infracao.IsAutuada", 1, (Model.Infracao.IsAutuada == null ? false : Model.Infracao.IsAutuada.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsAutuadaSim" }))%>Grave</label>
			    <label><%= Html.RadioButton("Infracao.IsAutuada", 0, (Model.Infracao.IsAutuada == null ? false : !Model.Infracao.IsAutuada.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsAutuadaNao" }))%>Gravíssima</label>
            </div>
        </div>
    </fieldset>

    <fieldset class="box fsInfracao fsPenalidade">
        <legend>Penalidade</legend>

        <label>Enquadramento da penalidade conforme Lei 10.476/2015</label>

        <div class="block"  style="padding-top: 5px;">
            <div class="block coluna30">
                <label><%= Html.CheckBox("Penalidade.Item", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeItem1" }))%>Art.2º Item I - Advertência</label><br />
                <label><%= Html.CheckBox("Penalidade.Item", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeItem2" }))%>Art.2º Item II - Multa</label><br />
                <label><%= Html.CheckBox("Penalidade.Item", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeItem3" }))%>Art.2º Item III - Apreensão</label><br />
                <label><%= Html.CheckBox("Penalidade.Item", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeItem4" }))%>Art.2º Item IV - Interdição ou embargo</label>
            </div>

            <div class="block coluna65">

                <%foreach(var item in Model.Penalidades) { %>
                    <input type="hidden" class="hdnPenalidade<%:item.Id%>" value="<%:item.Codigo %>" />
                <% } %>

                <%--<input type="hidden" class="hdnPenalidade" value="<%:ViewModelHelper.Json(Model.Penalidades) %>" />--%>

                <div class="block">
                    <div class="coluna2 append2">
                        <%= Html.CheckBox("Penalidade.Item", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeItem5" }))%>
                    </div>
                    <div class="coluna30 append2">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.ListaPenalidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeItem5" }))%>
                    </div>
                    <div class="coluna50">
                        <%= Html.TextBox("Penalidade.Descricao", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeItem5" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2">
                        <%= Html.CheckBox("Penalidade.Item", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeItem6" }))%>
                    </div>
                    <div class="coluna30 append2">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.ListaPenalidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeItem6" }))%>
                    </div>
                    <div class="coluna50">
                        <%= Html.TextBox("Penalidade.Descricao", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeItem6" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2">
                        <%= Html.CheckBox("Penalidade.Item", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeItem7" }))%>
                    </div>
                    <div class="coluna30 append2">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.ListaPenalidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeItem7" }))%>
                    </div>
                    <div class="coluna50">
                        <%= Html.TextBox("Penalidade.Descricao", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeItem7" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2">
                        <%= Html.CheckBox("Penalidade.Item", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeItem8" }))%>
                    </div>
                    <div class="coluna30 append2">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.ListaPenalidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeItem8" }))%>
                    </div>
                    <div class="coluna50">
                        <%= Html.TextBox("Penalidade.Descricao", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeItem8" }))%>
                    </div>
                </div>
            </div>
        </div>
    </fieldset>


</div>