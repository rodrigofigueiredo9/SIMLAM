<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ListarRelatorios
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="central">

        <h1 class="titTela">Listar Relatórios Personalizados</h1>

        <!-- FILTRO ============= -->
        <section class="block">
	        <div class="filtroExpansivo">
		        
		        <div class="filtroCorpo block">
			        <div class="coluna44">
				        <p>
					        <label for="dummy3">Tipo do Relatório</label><br>
					        <select name="dummy3">
						        <option>Titulo</option>	
					        </select>
				        </p>
			        </div>
			        <div class="coluna42">
				        <p>
					        <label for="loginFuncionario">Título do Relatório</label>
					        <input type="text" class="text" id="loginFuncionario" name="loginFuncionario">
				        </p>
			        </div>
			        <div class="coluna12 ultima">
				        <p>
					        <button class="inlineBotao">Buscar</button>
				        </p>
			        </div>
		        </div>
	        </div>
        </section>
        <!-- =================== -->

        <div class="block">
	        <div class="coluna100">
		        <h6 class="borderB padding0">Total de  <strong>1</strong> itens encontrados.</h6>
	        </div>				
        </div>

        <div class="itemListaLarga block posRelative margem0">
	        <div class="coluna80 ultima border">
		        <p class="margem0"><strong>Relatório de Títulos com Vencimento</strong></p>
		        <p class="quiet">Relatório de licença e pareceres emitidos pelo órgão com a sua respectiva data de vencimento.</p>
	        </div>
	        <div class="iconesListaRelatorios">
		        <a class="icone opcoes" title="Gerar Relatório">Gerar Relatório</a>
		        <a class="icone editar" title="Editar">Editar</a>
		        <a class="icone excluir" title="Excluir">Excluir</a>
		        <a href="titulo_relatorio.zip" class="icone selecEmpreendimento" title="Exportar Relatório">Exportar Relatório</a>
	        </div>
        </div>

        <div class="itemListaLarga block posRelative margem0">
	        <div class="coluna80 ultima border">
		        <p class="margem0"><strong>Relatório de Títulos com Vencimento</strong></p>
		        <p class="quiet">Relatório de licença e pareceres emitidos pelo órgão com a sua respectiva data de vencimento.</p>
	        </div>
	        <div class="iconesListaRelatorios">
		        <a class="icone opcoes" title="Gerar Relatório">Gerar Relatório</a>
		        <a class="icone editar" title="Editar">Editar</a>
		        <a class="icone excluir" title="Excluir">Excluir</a>
		        <a href="titulo_relatorio.zip" class="icone selecEmpreendimento" title="Exportar Relatório">Exportar Relatório</a>
	        </div>
        </div>

    </div>
</asp:Content>
