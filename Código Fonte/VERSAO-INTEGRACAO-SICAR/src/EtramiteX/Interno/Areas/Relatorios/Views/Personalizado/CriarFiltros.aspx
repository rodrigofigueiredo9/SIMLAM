<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">CriarFiltros</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
    <script language="javascript" type="text/javascript">
     <!--
        $(function () {
            ///Deixa a largura dos itens da Wizard distribuida de forma igual
            $('.wizBar ul li').css('width', 100 / $('.wizBar li').length + '%');


            if ($.browser.msie) { ///IE SUCKS!!! flag que vai deletar os elementos no evento de STOP do Sortable
                deleteMe = false;
            }

            ///Inicia "Draggable/Sortable/Droppable" dos elementos
            $('.areaFiltroMultiplo').sortable({
                revert: 50,
                placeholder: 'ui-state-highlight',
                receive: function (event, ui) {
                    $(this).find('span').removeClass('ui-button-text')
                },
                start: function (event, ui) {
                    ui.placeholder.width(ui.item.width());

                    if ($.browser.msie) { ///IE SUCKS!!! reseta a flag
                        deleteMe = false;
                    }
                },
                stop: function (event, ui) {
                    ui.item.removeClass('dell');

                    if ($.browser.msie) { ///IE SUCKS!!! delata o item se tiver a flag
                        if (deleteMe) {
                            ui.item.remove();
                        }
                    }
                },
                containment: 'document',
                scope: 'filtros'
            });

            $('.dragButtons li').draggable({
                connectToSortable: '.areaFiltroMultiplo',
                helper: 'clone',
                revert: 'invalid',
                revertDuration: 50,
                stop: function (event, ui) {
                    $('.areaFiltroMultiplo').sortable("option", "revert", 50);
                },
                containment: 'document'
            });
            $('div.dell').droppable({
                drop: function (event, ui) {
                    ui.helper.remove();

                    if ($.browser.msie) { ///IE SUCKS!!! coloca a flag para o item ser deletado
                        deleteMe = true;
                    }
                },
                activeClass: 'active',
                activate: function (event, ui) {
                    $('.dellLegend').text('Arraste nessa área para deletar o item');
                },
                deactivate: function (event, ui) {
                    $('.dellLegend').text('');
                },
                over: function (event, ui) {
                    $('.dellLegend').text('Deletar o Item');
                    ui.helper.addClass('remover');
                },
                out: function (event, ui) {
                    $('.dellLegend').text('Arraste nessa área para deletar o item');
                    ui.helper.removeClass('remover');
                },
                scope: 'filtros'
            });


            $('ul, li').disableSelection(); ///Desabilita a seleção de textos dos elementos

            ///Deletar elementos
            $('.areaFiltroMultiplo li').live('click', function (e) {
                $(this).toggleClass('dell');
            });
            $('div.dell').live('click', function (e) {
                $('.areaFiltroMultiplo li.dell').remove();
            });


            //Botões Jquery
            $('.dragButtons li').button();

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
            $('.btAdicionar').button({
                icons: {
                    primary: 'ui-icon-plusthick'
                },
                text: false
            });
        });
    //-->
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="central">

        <h1 class="titTela">Criar Relatórios Personalizados</h1>

        <!-- ========================================================================= -->
        <div class="block margem0">
            <div class="wizBar coluna100">
                <ul>
                    <li class="anterior"><span>Opções</span></li>
                    <li class="anterior"><span>Ordenar Colunas</span></li>
                    <li class="anterior"><span>Ordenar Valores</span></li>
                    <li class="ativo"><span>Filtros</span></li>
                    <li class=""><span>Sumarização</span></li>
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

        <h5>Filtro</h5>

        <div class="block borderB padding0">
            <div class="coluna20">
                <p>
                    <label for="dummy1">Campo</label>
                    <select name="dummy1">

                        <option>Titulo - Autor</option>
                        <option>Titulo - Número</option>
                        <option>Titulo - Data de conclusão</option>
                        <option>Titulo - Motivo do encerramento</option>
                        <option>Titulo - Setor</option>
                        <option>Titulo - Data de emissão</option>
                        <option>Titulo - Dias prorrogados</option>
                        <option>Titulo - Prazo</option>
                        <option>Titulo - Data de Criação</option>
                        <option>Titulo - Data de Assinatura</option>
                        <option>Titulo - Data de Encerramento</option>
                        <option>Titulo - Data de Vencimento</option>
                        <option>Titulo - Situação</option>
                        <option>Titulo - Local da Emissão</option>

                        <option>Modelo - Nome</option>
                        <option>Modelo - Subtipo</option>
                        <option>Modelo - Sigla</option>
                        <option>Modelo - Situação</option>

                        <option>Processo - Nome</option>
                        <option>Processo - Tipo</option>
                        <option>Processo - Data de Criação</option>

                        <option>Documento - Número</option>
                        <option>Documento - Tipo</option>
                        <option>Documento - Data de Criação</option>
                        <option>Documento - Nome</option>

                        <option>Empreendimento - Razão Social</option>
                        <option>Empreendimento - Atividade Principal</option>
                        <option>Empreendimento - Segmento</option>
                        <option>Empreendimento - Área Total</option>
                        <option>Empreendimento - Nome Fansasia</option>
                        <option>Empreendimento - CNPJ</option>

                        <option>Interessados - Nome</option>
                        <option>Interessados - Razão Social</option>
                        <option>Interessados - CPF</option>
                        <option>Interessados - CNPJ</option>

                        <option>Data de Inicio do Prazo - Data</option>
                        <option>Data de Inicio do Prazo - Dia</option>
                        <option>Data de Inicio do Prazo - Mês</option>
                        <option>Data de Inicio do Prazo - Ano</option>
                    </select>
                </p>
            </div>

            <div class="coluna15">
                <p>
                    <label for="dummy2">Operador</label>
                    <select name="dummy2">
                        <option>Igual</option>
                        <option>Diferente</option>
                        <option>Maior</option>
                        <option>Menor</option>

                    </select>
                </p>
            </div>

            <div class="coluna30">
                <p>
                    <label for="dummy3">Filtro</label>
                    <input type="text" class="text" id="" name="dummy3">
                </p>
            </div>
            <div class="coluna20">
                <p class="paddingT20">
                    <label class="labelCheckBox"><input type="checkbox"> <span>Definir na Execução</span></label>
                </p>
            </div>
            <div class="coluna11 ultima">
                <button class="btAdicionar floatRight inlineBotao" title="Adicionar">Adicionar</button>

            </div>
        </div>

        <h5 class="margemDTop">Construtor de Filtro Multiplo</h5>

        <div class="block">
            <div class="coluna60">
                <ul class="dragButtons">
                    <li class="adicional" title="E">E</li>
                    <li class="adicional" title="OU">OU</li>
                    <li class="abreParenteses" title="Abre Parênteses">(</li>
                    <li class="fechaParenteses" title="Fecha Parênteses">)</li>
                </ul>
                <span class="dica quiet margemEsq">Arraste os operadores para montar o filtro.</span>
            </div>

        </div>
        <div class="block">


        <!-- ========================================================================= -->
        <!-- ========================================================================= -->
            <ul class="areaFiltroMultiplo">

                <li class="filtro"><span>Título – Situação </span><em title="Igual">Igual</em><span class="" title="legenda do campo de execução">Concluido</span></li>
                <li class="adicional">E</li>
                <li class="abreParenteses">(</li>
                <li class="filtro"><span>Modelo - Nome</span><em title="Diferente">Diferente</em><span>Licença Prévia</span></li>
                <li class="adicional">OU</li>
                <li class="filtro"><span>Modelo - Nome</span><em title="Menor que">Menor que</em><span>Licença de Instalação</span></li>
                <li class="adicional">OU</li>
                <li class="filtro"><span>Modelo - Nome</span><em title="Maior que">Maior que</em><span>Licença de Operação</span></li>
                <li class="adicional">OU</li>
                <li class="filtro"><span>Modelo - Nome</span><em title="Maior ou Igual a">Maior ou Igual a</em><span>Parecer Técnico</span></li>
                <li class="filtro"><span>Modelo - Nome</span><em title="Menor ou Igual a">Menor ou Igual a</em><span>Parecer Técnico</span></li>
                <li class="filtro"><span>Modelo - Nome</span><em title="Não é Nulo">Não é Nulo</em><span>Parecer Técnico</span></li>
                <li class="filtro"><span>Modelo - Nome</span><em title="É Nulo">É Nulo</em><span>Parecer Técnico</span></li>
                <li class="fechaParenteses">)</li>

            </ul>
        <!-- ========================================================================= -->
            <!--
			    Lista de Icones de Operadore para referencia rápida:
			    <em title="Igual">Igual</em>
			    <em title="Diferente">Diferente</em>
			    <em title="Menor que">Menor que</em>
			    <em title="Maior que">Maior que</em>
			    <em title="Maior ou Igual a">Maior ou Igual a</em>
			    <em title="Menor ou Igual a">Menor ou Igual a</em>
			    <em title="Não é Nulo">Não é Nulo</em>
			    <em title="É Nulo">É Nulo</em>
		    -->
        <!-- ========================================================================= -->

           <!--<button class="dell"><span>Deletar Item</span></button>-->
            <div class="dell"><span></span></div><span class="dellLegend"></span>
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
