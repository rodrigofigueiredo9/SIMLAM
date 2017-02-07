<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMUnidadeProducao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UnidadeProducaoVM>" %>

<div class="box">
	<%=Html.Hidden("UnidadeProducaoId", Model.UnidadeProducao.Id, new { @class="hdnUnidadeProducaoId" })%>
	<%=Html.Hidden("EmpreendimentoId", Model.UnidadeProducao.Empreendimento.Id, new { @class="hdnEmpreendimentoId" })%>
	<%=Html.Hidden("ProjetoDigitalId", Request.Params["ProjetoDigitalId"], new { @class="hdnProjetoDigitalId" })%>

	<div class="block">
		<div class="coluna29">
			<label for="Possui_Codigo_Propriedade">Já possui código da propriedade? *</label><br />
			<label><%= Html.RadioButton("UnidadeProducao.PossuiCodigoPropriedade", true, Model.UnidadeProducao.PossuiCodigoPropriedade, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.UnidadeProducao.CodigoPropriedade > 0), new { @class = "radio RadioPropriedadeCodigo rbPossuiCodigoSim" }))%>Sim</label>
			<label><%= Html.RadioButton("UnidadeProducao.PossuiCodigoPropriedade", false, !Model.UnidadeProducao.PossuiCodigoPropriedade, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.UnidadeProducao.CodigoPropriedade > 0), new { @class = "radio RadioPropriedadeCodigo rbPossuiCodigoNao" }))%>Não</label>
		</div>
		<div class="coluna25 prepend1">
			<label for="Codigo_Propriedade">Código da propriedade *</label>
			<%= Html.TextBox("UnidadeProducao.CodigoPropriedade", (Model.UnidadeProducao.CodigoPropriedade > 0 ? Model.UnidadeProducao.CodigoPropriedade.ToString() : "Gerado automaticamente"), ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.UnidadeProducao.InternoID > 0 || !Model.UnidadeProducao.PossuiCodigoPropriedade, new { @class = "text txtCodigoPropriedade maskNumInt", @maxlength = "11" }))%>
		</div>
		<div class="coluna25 prepend1">
			<label for="Codigo_Empreendimento">Código do empreendimento *</label>
			<%= Html.TextBox("UnidadeProducao.Empreendimento.Codigo", Model.UnidadeProducao.Empreendimento.Codigo, new { @class = "text txtCodigoEmpreendimento disabled", @maxlength = "15", @disabled="disabled" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label>Local em que o livro estará disponível *</label>
			<%= Html.TextBox("UnidadeProducao.LocalLivroDisponivel", Model.UnidadeProducao.LocalLivroDisponivel, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtLocalLivroDisponivel", @maxlength = "100" }))%>
		</div>
	</div>
</div>

<fieldset id="Unidades_Producao" class="box">
	<legend>Unidades de produção</legend>
	<%if(!Model.IsVisualizar){ %>
	<div class="block">
		<div class="coluna100">
			<button class="inlineBotao btnAdicionarUnidadeProducao direita" type="button" value="Adicionar"><span>Adicionar</span></button>
		</div>
	</div>
	<%} %>
	<div class="block">
		<table class="dataGridTable gridUnidadeProducao" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="16%">Código da UP</th>
					<th width="12%">Área(ha)</th>
					<th>Cultivar</th>
					<th width="14%">Quantidade/Ano</th>
					<th width="10%">Ações</th>
				</tr>
			</thead>
			<tbody>
				<%foreach (var item in Model.UnidadeProducao.UnidadesProducao){%>
				<tr>
					<td>
						<label class="lblCodigoUp" title="<%=item.CodigoUP < 1 ? "Gerado automaticamente" : item.CodigoUP.ToString() %>"><%=item.CodigoUP < 1 ? "Gerado automaticamente" : item.CodigoUP.ToString() %> </label>
					</td>
					<td>
						<label class="lblAreaHa"  title="<%=item.AreaHA.ToStringTrunc(4) %>"><%= item.AreaHA.ToStringTrunc(4) %> </label>
					</td>
					<td>
						<label class="lblCultura" title="<%=item.CulturaTexto %>"><%=item.CulturaTexto%>
						<%if(!string.IsNullOrEmpty(item.CultivarTexto)){ %>
							<%=" " + item.CultivarTexto%> 
						<%} %>
						</label>
					</td>
					<td>
						<label class="lblEstimativaQuantidadeAno" title="<%=item.EstimativaProducaoQuantidadeAno.ToStringTrunc(4) + item.EstimativaProducaoUnidadeMedida %>">
							<%=item.EstimativaProducaoQuantidadeAno.ToStringTrunc(4) + " " +item.EstimativaProducaoUnidadeMedida%> 
						</label>
					</td>
					<td>
						<%if(!Model.IsVisualizar){ %> <a class="icone excluir btnExcluirUnidadeProducao"></a><%} %>
						<%if(!Model.IsVisualizar){ %> <a class="icone editar btnEditarUnidadeProducao"></a><%} %>
						<%if(Model.IsVisualizar){ %> <a class="icone visualizar btnVisualizarUP"></a><%} %>
						<input type="hidden" value="<%=item.Id %>" class="hdnItemId" />
						<input type="hidden" value='<%=ViewModelHelper.Json(item) %>' class="hdnItemObjeto" />
					</td>
				</tr>
				<% } %>
				<tr class="trTemplate hide">
					<td>
						<label class="lblCodigoUp" title=""></label>
					</td>
					<td>
						<label class="lblAreaHa" title=""></label>
					</td>
					<td>
						<label class="lblCultura" title=""></label>
					</td>
					<td>
						<label class="lblEstimativaQuantidadeAno" title=""> </label>
					</td>
					<td>
						<a class="icone excluir btnExcluirUnidadeProducao"></a>
						<a class="icone editar btnEditarUnidadeProducao"></a>
						<input type="hidden" value="" class="hdnItemId" />
						<input type="hidden" value="" class="hdnItemObjeto" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>