<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    CriarSumarizacao
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">

	<script language="javascript" type="text/javascript">
	<!--
	    $(function () {
	        ///Deixa a largura dos itens da Wizard distribuida de forma igual
	        $('.wizBar ul li').css('width', 100 / $('.wizBar li').length + '%');


	        //Botões Jquer
	        $('.btAvancar').button({
	            icons: {
	                secondary: 'ui-icon-avancar'
	            }
	        });
	        $('.btVoltar').button({
	            icons: {
	                primary: 'ui-icon-voltar'
	            }
	        });
	    });
	//-->
	</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<!-- SEÇÃO CENTRAL ========================================================== -->
			<div id="central">

				<h1 class="titTela">Criar Relatórios Personalizados</h1>
				
				<!-- ========================================================================= -->
				<!-- ========================================================================= -->
				<div class="block margem0">
					<div class="wizBar coluna100">
						<ul>
							<li class="anterior"><span>Opções</span></li>
							<li class="anterior"><span>Ordenar Colunas</span></li>
							<li class="anterior"><span>Ordenar Valores</span></li>
							<li class="anterior"><span>Filtros</span></li>
							<li class="ativo"><span>Sumarização</span></li>
							<li class=""><span>Dimensionar</span></li>
							<li class=""><span>Agrupar</span></li>
							<li class=""><span>Salvar</span></li>
						</ul>
					</div><!-- .wizBar -->
				</div>
				<!-- ========================================================================= -->
				<!-- ========================================================================= -->
				
				<!-- navegaçâo da wizard ===================================================== -->
				<div class="block box">
					<div class="coluna25 floatRight alinhaDireita">
						<button class="btAvancar">Avançar</button>
					</div>
					<div class="coluna25">
						<button class="btVoltar">Voltar</button>
					</div>
				</div>
				<!-- ========================================================================= -->				
				<!-- ========================================================================= -->
				
				<div class="block">
					<div class="coluna25">
						<label class="labelCheckBox"><input type="checkbox"> <span>Contar número de registros</span></label>
					</div>
				</div>
				<div class="block">
					<table width="100%" border="0" cellspacing="0" cellpadding="0" class="dataGridTable ordenavel">
						<thead>
							<tr>
								<th width="50%">Coluna</th>
								<th width="7%">Contar</th>
								<th width="7%">Soma</th>
								<th width="7%">Média</th>
								<th>Valor Máximo</th>
								<th>Valor Mínimo</th>
							</tr>
						</thead>
						
						<tbody>
							<tr>
								<td>Empreendimento – Área total </td>
								<td><input type="checkbox"></td>
								<td><input CHECKED type="checkbox"></td>
                                                                <td><input  type="checkbox"></td>
								<td><input type="checkbox"></td>
								<td><input type="checkbox"></td>
							</tr>
							
					</table>					
									
				</div>

				<!-- ========================================================================= -->
				<!-- navegaçâo da wizard ===================================================== -->
					
				<div class="block box margemDTop">
					<div class="coluna25 floatRight alinhaDireita">
						<span class="cancelarCaixaDir">ou <a class="linkCancelar" title="Cancelar">Cancelar</a></span>
						<button class="btAvancar">Avançar</button>
					</div>
					<div class="coluna25">
						<button class="btVoltar">Voltar</button>
					</div>
				</div>
				<!-- ========================================================================= -->
				

			</div>
</asp:Content>
