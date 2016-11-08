<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResultadoMergeItemVM>" %>

<script type="text/javascript">
	ModalAtualizarRoteiro.itensJSON = <%= ViewModelHelper.JsSerializer.Serialize(Model.MergeItens.Itens) %>;
	ModalAtualizarRoteiro.roteirosJSON = <%= ViewModelHelper.JsSerializer.Serialize(Model.ObterRoteirosSimplificado()) %>;
</script>

<div class="divMensagemRoteiro">
</div>
<h1 class="titTela">Atualizar Itens de Análise</h1>
<br />

<div class="divAtualizarItensAnalise">
	<fieldset class="block box">
		<legend></legend>
		<div>
			<div class="dataGrid">
				<table class="tabRoteiros tbItens dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Itens atuais na análise</th>
							<th>Itens na análise após a atualização</th>
							<th width="20%">Tipo de atualização do item</th>
						</tr>
					</thead>
					<tbody>
						<%
							foreach (var item in Model.MergeItens.ItensAtualizados.OrderBy(x => x.StatusId)) 
						{ %>
						<tr>
						
							<% string nomeItem = string.Empty;

							foreach (var itemAtuais in Model.MergeItens.ItensAtuais)
							{
								if (item.Id == itemAtuais.Id)
								{
									nomeItem = itemAtuais.Nome;
									break;
								}
							}
							%>
							<td>
								<span class="trRoteiroNumero " title="<%= Html.Encode(nomeItem) %>"><%= Html.Encode(nomeItem)%></span>
							</td>
							<td>
								<span class="trRoteiroNumero " title="<%= Html.Encode(item.Nome) %>"><%= Html.Encode(item.Nome) %></span>
							</td>
							<td>
								<span class="trRoteiroVersao" title="<%= Html.Encode(item.StatusTela) %>"><%= Html.Encode(item.StatusTela)%></span>
								<input type="hidden" class="itemJSON" value='<%= ViewModelHelper.Json(item)  %>' />
							</td>
						</tr>
						<%} %>
					
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>
</div>