﻿<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
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
	    <legend>Caracterização da infração/fiscalização</legend>

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
			    <div class="coluna80">
                    <input type="hidden" class="enquadramentoId" value="<%=Model.Infracao.EnquadramentoInfracao.Id%>" />
				    <table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					    <thead>
						    <tr>
							    <th style="width:12%">Artigo</th>
                                <th style="width: 15%">Item/Parágrafo/Alínea</th>
                                <th>Lei/Decreto/Resolução/Portaria/Instrução Normativa</th>
						    </tr>
					    </thead>
					    <tbody>
					        <% for (int linha = 0; linha < 3; linha++) { %>
						        <tr id="Infracao_Enquadramento" >
                                    <td>
                                        <% if (!Model.IsVisualizar) { %>
                                            <input type="text" class="text txtArtigoEnquadramento" value ="<%=(Model.Infracao.EnquadramentoInfracao.Artigos.Count > linha ? Model.Infracao.EnquadramentoInfracao.Artigos[linha].ArtigoTexto : string.Empty)%>" maxlength="16" style="border:none; width:100%; background-color:transparent; margin-bottom:1px; align-self:baseline; text-align:center;" />
                                        <% } else { %>
                                            <input type="text" class="text txtArtigoEnquadramento disabled" disabled="disabled" value ="<%=(Model.Infracao.EnquadramentoInfracao.Artigos.Count > linha ? Model.Infracao.EnquadramentoInfracao.Artigos[linha].ArtigoTexto : string.Empty)%>" maxlength="16" style="border:none; width:100%; background-color:transparent; margin-bottom:1px; align-self:baseline; text-align:center;" />
                                        <% } %>
                                    </td>
                                    <td>
                                        <% if (!Model.IsVisualizar) { %>
                                            <input type="text" class="text txtItemEnquadramento" value ="<%=(Model.Infracao.EnquadramentoInfracao.Artigos.Count > linha ? Model.Infracao.EnquadramentoInfracao.Artigos[linha].ArtigoParagrafo : string.Empty)%>" maxlength="16" style="border: none; width:100%; background-color:transparent; margin-bottom:1px; align-self:baseline; text-align:center;" />
                                        <% } else { %>
                                            <input type="text" class="text txtItemEnquadramento disabled" disabled="disabled" value ="<%=(Model.Infracao.EnquadramentoInfracao.Artigos.Count > linha ? Model.Infracao.EnquadramentoInfracao.Artigos[linha].ArtigoParagrafo : string.Empty)%>" maxlength="16" style="border: none; width:100%; background-color:transparent; margin-bottom:1px; align-self:baseline; text-align:center;" />
                                        <% } %>
                                    </td>
                                    <td>
                                        <% if (!Model.IsVisualizar) { %>
                                            <input type="text" class="text txtLeiEnquadramento" value ="<%=(Model.Infracao.EnquadramentoInfracao.Artigos.Count > linha ? Model.Infracao.EnquadramentoInfracao.Artigos[linha].DaDo : string.Empty)%>" maxlength="89" style="border: none; width:100%; background-color:transparent; margin-bottom:1px; align-self:baseline;" />
                                        <% } else { %>
                                            <input type="text" class="text txtLeiEnquadramento disabled" disabled="disabled" value ="<%=(Model.Infracao.EnquadramentoInfracao.Artigos.Count > linha ? Model.Infracao.EnquadramentoInfracao.Artigos[linha].DaDo : string.Empty)%>" maxlength="89" style="border: none; width:100%; background-color:transparent; margin-bottom:1px; align-self:baseline;" />
                                        <% } %>
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
			    <%= Html.TextArea("Infracao.DescricaoInfracao", Model.Infracao.DescricaoInfracao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtDescricaoInfracao", @maxlength = "940" }))%>
		    </div>
	    </div>

        <div class="block">
            <div class="coluna20 append2">
			    <label>Data da constatação/vistoria *</label>
			    <%= Html.TextBox("Infracao.DataConstatacao", Model.Infracao.DataConstatacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataConstatacao" }))%>
		    </div>

            <div class="coluna20 append2">
			    <label>Hora da constatação/vistoria *</label>
			    <%= Html.TextBox("Infracao.HoraConstatacao", Model.Infracao.HoraConstatacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskHoraMinuto txtHoraConstatacao" }))%>
		    </div>

            <div class="coluna40 divClassificacao">
                <label>Classificação da infração *</label>
                <br />
                <label><%= Html.RadioButton("Infracao.ClassificacaoInfracao", 0, (Model.Infracao.ClassificacaoInfracao == null ? false : Model.Infracao.ClassificacaoInfracao.Value == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoClassificacaoLeve" }))%>Leve</label>
			    <label><%= Html.RadioButton("Infracao.ClassificacaoInfracao", 1, (Model.Infracao.ClassificacaoInfracao == null ? false : Model.Infracao.ClassificacaoInfracao.Value == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoClassificacaoMedia" }))%>Média</label>
                <label><%= Html.RadioButton("Infracao.ClassificacaoInfracao", 2, (Model.Infracao.ClassificacaoInfracao == null ? false : Model.Infracao.ClassificacaoInfracao.Value == 2), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoClassificacaoGrave" }))%>Grave</label>
			    <label><%= Html.RadioButton("Infracao.ClassificacaoInfracao", 3, (Model.Infracao.ClassificacaoInfracao == null ? false : Model.Infracao.ClassificacaoInfracao.Value == 3), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoClassificacaoGravissima" }))%>Gravíssima</label>
            </div>
        </div>
    </fieldset>

    <fieldset class="box fsInfracao fsPenalidade">
        <legend>Penalidade</legend>

        <label>Enquadramento da penalidade conforme Lei 10.476/2015</label>

        <div class="block"  style="padding-top: 15px;">
            <div class="block coluna25">
                <div class="block" style="height:28px; align-self:center;">
                    <label><%= Html.CheckBox("Penalidade.Item", (Model.Infracao.PossuiAdvertencia == null ? false : Model.Infracao.PossuiAdvertencia.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeAdvertencia" }))%>Art.2º Item I - Advertência</label><br />
                </div>
                <div class="block" style="height:28px; align-self:center;">
                    <label><%= Html.CheckBox("Penalidade.Item", (Model.Infracao.PossuiMulta == null ? false : Model.Infracao.PossuiMulta.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeMulta" }))%>Art.2º Item II - Multa</label><br />
                </div>
                <div class="block" style="height:28px; align-self:center;">
                    <label><%= Html.CheckBox("Penalidade.Item", (Model.Infracao.PossuiApreensao == null ? false : Model.Infracao.PossuiApreensao.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeApreensao" }))%>Art.2º Item III - Apreensão</label><br />
                </div>
                <div class="block" style="height:28px; align-self:center;">
                    <label><%= Html.CheckBox("Penalidade.Item", (Model.Infracao.PossuiInterdicaoEmbargo == null ? false : Model.Infracao.PossuiInterdicaoEmbargo.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeInterdicaoEmbargo" }))%>Art.2º Item IV - Interdição ou embargo</label>
                </div>
            </div>

            <div class="coluna1" style="border-left: 2px solid; height:130px;">
                <%--Linha vertical--%>
            </div>

            <div class="block coluna60">

                <%foreach(var item in Model.Penalidades) { %>
                    <input type="hidden" class="hdnPenalidade<%:item.Id%>" value="<%:item.Codigo %>" />
                <% } %>

                <div class="block" style="height:28px; align-self:center;">
                    <div class="coluna2 append2">
                        <%= Html.CheckBox("Penalidade.Item", (Model.Infracao.IdsOutrasPenalidades[0] != 0 ? true : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna30 append2" style="height:28px; align-self:center;">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.ListaPenalidades01, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna50" style="height:28px; align-self:center;">
                        <%= Html.TextBox("Penalidade.Descricao", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2" style="height:28px; align-self:center;">
                        <%= Html.CheckBox("Penalidade.Item", (Model.Infracao.IdsOutrasPenalidades[1] != 0 ? true : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna30 append2" style="height:28px; align-self:center;">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.ListaPenalidades02, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna50" style="height:28px; align-self:center;">
                        <%= Html.TextBox("Penalidade.Descricao", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2" style="height:28px; align-self:center;">
                        <%= Html.CheckBox("Penalidade.Item", (Model.Infracao.IdsOutrasPenalidades[2] != 0 ? true : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna30 append2" style="height:28px; align-self:center;">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.ListaPenalidades03, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna50" style="height:28px; align-self:center;">
                        <%= Html.TextBox("Penalidade.Descricao", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2" style="height:28px; align-self:center;">
                        <%= Html.CheckBox("Penalidade.Item", (Model.Infracao.IdsOutrasPenalidades[3] != 0 ? true : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbPenalidade cbPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna30 append2" style="height:28px; align-self:center;">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.ListaPenalidades04, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna50" style="height:28px; align-self:center;">
                        <%= Html.TextBox("Penalidade.Descricao", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>
            </div>
        </div>
    </fieldset>


</div>