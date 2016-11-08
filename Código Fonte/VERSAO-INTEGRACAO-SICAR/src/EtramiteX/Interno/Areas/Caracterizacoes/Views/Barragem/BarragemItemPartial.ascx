<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemItemVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<div class="divBarragemItem">
	<input type="hidden" class="hdnBarragemItemId" value="<%: Model.BarragemItem.Id %>" />
	<fieldset class="block box">	
		<legend class="titFiltros">Barragem</legend>
		<div class="block">
			<div class="coluna25 append2">
				<label for="BarragemItem.Quantidade">Quantidade de barragens *</label>
				<%= Html.TextBox("BarragemItem.Quantidade", Model.BarragemItem.Quantidade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtQuantidade maskNum3", @id = "txtQuantidade", @maxlength = 3 }))%>
			</div>
			<div class="coluna25 append2">
				<label for="BarragemItem.FinalidadeId">Finalidade *</label>
				<%= Html.DropDownList("BarragemItem.FinalidadeId", Model.Finalidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlFinalidade", @id = "ddlFinalidade" }))%>
			</div>
			<div class="coluna24 divEspecificar<%= Model.BarragemItem.FinalidadeId > 0 && Model.BarragemItem.FinalidadeId == Model.FinalidadeOutrosId ? " " : " hide" %>">
				<label for="BarragemItem.Especificar">Especificar *</label>
				<%= Html.TextBox("BarragemItem.Especificar", Model.BarragemItem.Especificar, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEspecificar", @id = "txtEspecificar", @maxlength = 30 }))%>
			</div>
		</div>

		<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna13 append2">
				<label for="txtIdentificador">Identificador *</label>
				<%= Html.TextBox("txtIdentificador", "", ViewModelHelper.SetaDisabled(true, new { @class = "text txtIdentificador", @id = "txtIdentificador" }))%>
			</div>
			<div class="coluna28 append2">
				<label for="txtLaminaAgua">Lâmina d'água por barragem (ha)*</label>
				<%= Html.TextBox("txtLaminaAgua", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtLaminaAgua maskDecimal4", @id = "txtLaminaAgua", @maxlength = 14 }))%>
			</div>
			<div class="coluna33 append2">
				<label for="txtArmazenado">Volume armazenado por barragem (m³)*</label>
				<%= Html.TextBox("txtArmazenado", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArmazenado maskDecimal4", @id = "txtArmazenado", @maxlength = 14 }))%>
			</div>
			<div class="coluna15 botaoAddBarragemItemDados">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAddBarragemItemDados " title="Adicionar dados da barragem">Adicionar</button>		  
			</div>
		</div>
		<div class="block divOutorga<%= Model.BarragemItem.FinalidadeId > 0 && Model.BarragemItem.FinalidadeId != Model.FinalidadeReservacaoId ? " " : " hide" %>">
			<div class="coluna21 append2">
				<label for="ddlOutorga">Outorga de uso da água</label>
				<%= Html.DropDownList("ddlOutorga", Model.Outorgas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlOutorga", @id = "ddlOutorga" }))%>
			</div>
			<div class="coluna20">
				<label for="txtNumero">Numero</label>
				<%= Html.TextBox("txtNumero", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumero", @id = "txtNumero", @maxlength = 15 }))%>
			</div>
		</div>
		<% } %>
		<div class="divGridBarragemItemDados block">
			<table class="dataGridTable gridBarragemItemDados" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="10%">Identificador</th>
						<th width="31%">Área de lâmina d´água por barragem (ha)</th>
						<th width="19%">Volume armazenado (m³)</th>
						<th>Outorga de água</th>
						<th width="15%">Número da Outorga</th>
						<% if (!Model.IsVisualizar) { %>
						<th width="5%">Ações</th>
						<% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.BarragemItem.BarragensDados) { %>
					<tr>
						<td><span class="spanIdentificador" title="<%= item.Identificador  %>"><%= item.Identificador  %></span></td>
						<td><span class="spanLaminaAgua" title="<%= item.LaminaAguaToDecimal.ToStringTrunc(4) %>"><%= item.LaminaAguaToDecimal.ToStringTrunc(4) %></span></td>
						<td><span class="spanVolumeArmazenamento" title="<%= item.VolumeArmazenamentoToDecimal.ToStringTrunc(4) %>"><%= item.VolumeArmazenamentoToDecimal.ToStringTrunc(4) %></span></td>
						<td><span class="spanOutorgaTexto" title="<%= item.OutorgaTexto %>"><%= item.OutorgaTexto %></span></td>
						<td><span class="spanNumero" title="<%= item.Numero %>"><%= item.Numero %></span></td>
						<% if (!Model.IsVisualizar) { %>						
						<td>
							<input type="hidden" class="hdnItemBarragemItemDados" name="hdnItemBarragemItemDados" value='<%= Model.GetJSON(item) %>' />						
							<button title="Excluir" class="icone excluir btnExcluirLinhaBarragemItem" value="" type="button"></button>						
						</td>
						<% } %>
					</tr>
					<% } %>
				</tbody>
			</table>
			<% if (!Model.IsVisualizar) { %>
			<table style="display: none">
				<tbody>
					<tr class="trBarragemItemDadosTemplate">
						<td><span class="spanIdentificador" title=""></span></td>
						<td><span class="spanLaminaAgua" title=""></span></td>
						<td><span class="spanVolumeArmazenamento" title=""></span></td>
						<td><span class="spanOutorgaTexto" title=""></span></td>
						<td><span class="spanNumero" title=""></span></td>
						<td>
							<input type="hidden" class="hdnItemBarragemItemDados" name="hdnItemBarragemItemDados" value="" />
							<button title="Excluir" class="icone excluir btnExcluirLinhaBarragemItem" value="" type="button"></button>
						</td>
					</tr>
				</tbody>
			</table>
			<% } %>
		</div>
		<br />

		<div class="block">
			<div class="coluna38 append2">
				<label for="txtTotalLaminaItem">Área total da lâmina (ha)</label>
				<%= Html.TextBox("txtTotalLaminaItem", Model.BarragemItem.ToStringTotalLamina, ViewModelHelper.SetaDisabled(true, new { @class = "text txtTotalLaminaItem", @id = "txtTotalLaminaItem" }))%>
			</div>
			<div class="coluna39">
				<label for="txtTotalArmazenadoItem">Volume total armazenado (m³)</label>
				<%= Html.TextBox("txtTotalArmazenadoItem", Model.BarragemItem.ToStringTotalArmazenado, ViewModelHelper.SetaDisabled(true, new { @class = "text txtTotalArmazenadoItem", @id = "txtTotalArmazenadoItem" }))%>
			</div>
		</div>
		<br />

		<% Html.RenderPartial("CoordenadaAtividade", Model.CoordenadaAtividadeVM); %>

	</fieldset>
</div>