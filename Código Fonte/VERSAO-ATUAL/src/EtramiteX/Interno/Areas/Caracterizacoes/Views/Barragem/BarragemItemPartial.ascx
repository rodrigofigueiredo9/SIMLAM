<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemItemVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<div class="divBarragemItem">
	<input type="hidden" class="hdnBarragemItemId" value="<%: Model.BarragemItem.Id %>" />
    <input type="hidden" class="hdnModificacoesNaoSalvas" value="0" />
	<fieldset class="block box">	
		<legend class="titFiltros">Barragem</legend>
		<div class="block">
			<div class="coluna20 append2">
				<label for="BarragemItem.Quantidade">Quantidade de barragens *</label>
				<%= Html.TextBox("BarragemItem.Quantidade", Model.BarragemItem.Quantidade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtQuantidade maskNum3", @id = "txtQuantidade", @maxlength = 3 }))%>
			</div>
		</div>

		<% if (!Model.IsVisualizar) { %>
        <div class="block">
            <div class="coluna50 append2">
			    <div class="block" id="checkboxes">
		            <label>Selecione as finalidades*</label> 
                     <br /> 
                    <input type="checkbox" name="Reservação" class="CheckReservacao" value="1" /> Reservação 
                    <br /> 
                    <input type="checkbox" name="Captação para Irrigação" value="2" class="CaptacaoIrrigacao" /> Captação para Irrigação 
                    <br /> 
                    <input type="checkbox" name="Ecoturismo/Turismo Rural" value="4" class="Turismo" /> Ecoturismo/Turismo Rural 
                    <br /> 
                    <input type="checkbox" name="Dessedentação de Animais" value="5" class="Dessedentacao" />  Dessedentação de Animais
                    <br /> 
                    <input type="checkbox" name="Aquicultura" value="6" class="Aquicultura" /> Aquicultura 
                    <br /> 
                    <input type="checkbox" name="Captação para Abastecimento Industrial" value="7" class="CaptacaoAbastecimentoIndustrial" /> Captação para Abastecimento Industrial 
                    <br /> 
                    <input type="checkbox" name="Captação para abastecimento Público" value="8" class="CaptacaoAbastecimentoPublico" />Captação para abastecimento Público  
	            </div>
			</div>
        </div>
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
		<div class="block divOutorga hide" %>">
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
						<th width="8%">Identificador</th>
                        <th width="20%">Finalidade</th>
						<th width="20%">Área de lâmina d´água por barragem (ha)</th>
						<th width="13%">Volume armazenado (m³)</th>
						<th width="10%">Outorga de água</th>
						<th width="10%">Número da Outorga</th>
						<% if (!Model.IsVisualizar) { %>
						<th width="10%">Ações</th>
						<% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.BarragemItem.BarragensDados) { %>
					<tr>
                         <input type="hidden" class="hdnItemId" value="<%: item.Id %>" /> 
						<td><span class="spanIdentificador" title="<%= item.Identificador  %>"><%= item.Identificador  %></span></td>
						<td><span class="spanFinalidade" title="<%= item.FinalidadeTexto %>"><%= item.FinalidadeTexto %></span></td>
                        <td><span class="spanLaminaAgua" title="<%= item.LaminaAguaToDecimal.ToStringTrunc(4) %>"><%= item.LaminaAguaToDecimal.ToStringTrunc(4) %></span></td>
						<td><span class="spanVolumeArmazenamento" title="<%= item.VolumeArmazenamentoToDecimal.ToStringTrunc(4) %>"><%= item.VolumeArmazenamentoToDecimal.ToStringTrunc(4) %></span></td>
						<td><span class="spanOutorgaTexto" title="<%= item.OutorgaTexto %>"><%= item.OutorgaTexto %></span></td>
						<td><span class="spanNumero" title="<%= item.Numero %>"><%= item.Numero %></span></td>
						<% if (!Model.IsVisualizar) { %>						
						<td>
							<input type="hidden" class="hdnItemBarragemItemDados" name="hdnItemBarragemItemDados" value='<%= Model.GetJSON(item) %>' />						
							<button title="Editar" class="icone editar btnEditar btnEditarFinalidade" value="" type="button"></button>
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
                        <td><span class="spanFinalidade" title=""></span></td>
						<td><span class="spanLaminaAgua" title=""></span></td>
						<td><span class="spanVolumeArmazenamento" title=""></span></td>
						<td><span class="spanOutorgaTexto" title=""></span></td>
						<td><span class="spanNumero" title=""></span></td>
						<td>
							<input type="hidden" class="hdnItemBarragemItemDados" name="hdnItemBarragemItemDados" value="" />                            
                            <button title="Editar" class="icone editar btnEditar btnEditarFinalidade" value="" type="button"></button>
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