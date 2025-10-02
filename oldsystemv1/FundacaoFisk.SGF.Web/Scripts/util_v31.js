// Caixa de Diálogo para Exclusão de registros
var DIALOGO_CONFIRMAR = 0;
var DIALOGO_INFORMACAO = 1;
var DIALOGO_AVISO = 2;
var DIALOGO_ERRO = 3;
var dataHoje = new Date();
var dia = dataHoje.getDate();
var mes = dataHoje.getMonth();
var ano = dataHoje.getFullYear();
var hora = dataHoje.getHours();
var min = dataHoje.getMinutes();
var seg = dataHoje.getSeconds();

var TIPO_RETORNO_LINK_PESQUISA = 0;
var TIPO_RETORNO_LINK_INCLUSAO = 1;
var TIPO_RETORNO_LINK_EDICAO = 2;
var MASCULINO = 2, FEMININO = 1;

var PRODUTO = 0, PERIODO = 1, DIA = 2, SITUACAO = 3, ESTAGIO = 4, TPAVALIACAO = 5, TITULOS = 6, MES = 7, MENUS = 8, CURSO = 9, TPITEM = 10;


//Monta Máscara para um campo de data
function maskDate(field) {
    $(field).mask("99/99/9999");
}

function maskHour(field) {
    $(field).mask("99:99");
};

function maskHHMMSS(field) {
    $(field).mask("99:99:99");
};

/*
 * Função que mostra o gif de carregando na tela, enquanto os componentes do dojo são montados.
 */
function showCarregando() {
    return showCarregando(null);
}

function showCarregando(body) {
    var divCarregando = document.getElementById('divCarregando');
    var divWrapper = document.getElementById('body');

    if (hasValue(body))
        divWrapper = body;

    if (hasValue(divCarregando) && hasValue(divWrapper))
        if (divCarregando.style.display == 'none') {
            divWrapper.style.opacity = '0.6';
            divCarregando.style.display = '';
        }
        else {
            divWrapper.style.opacity = '100';
            divCarregando.style.display = 'none';
        }
}

function hideCarregando() {
    var divCarregando = document.getElementById('divCarregando');
    var divWrapper = document.getElementById('body');

    if (hasValue(divCarregando) && hasValue(divWrapper)) {
        divWrapper.style.opacity = '100';
        divCarregando.style.display = 'none';
    }
}

function loadPesqSexo(Memory, id) {
    try {
        var stateStore = new Memory({
            data: [
                    { name: "Todos", id: 0 },
                    { name: "Feminino", id: 1 },
                    { name: "Masculino", id: 2 },
					{ name: "Não Binário", id: 3 },
					{ name: "Prefiro não responder ou Neutro", id: 4 },
            ]
        });
        id.store = stateStore;
        id.set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}
/*
 * Método responsável por validar uma mascara de um campo do tipo data (dd/mm/yyyy):
 */
function verificaData(digData) {
    var bissexto = 0;
    var data = digData;
    var tam = data.length;
    if (tam == 10) {
        var concatenador = data.substr(2, 1);

        if (concatenador != '/')
            return false;
        concatenador = data.substr(5, 1);

        if (concatenador != '/')
            return false;

        var dia = data.substr(0, 2);
        var mes = data.substr(3, 2);
        var ano = data.substr(6, 4);
        if ((ano > 1900) || (ano < 2100)) {
            switch (mes) {
                case '01':
                case '03':
                case '05':
                case '07':
                case '08':
                case '10':
                case '12':
                    if (dia <= 31) {
                        return true;
                    }
                    break;

                case '04':
                case '06':
                case '09':
                case '11':
                    if (dia <= 30) {
                        return true;
                    }
                    break;
                case '02':
                    /* Validando ano Bissexto / fevereiro / dia */
                    if ((ano % 4 == 0) || (ano % 100 == 0) || (ano % 400 == 0)) {
                        bissexto = 1;
                    }
                    if ((bissexto == 1) && (dia <= 29)) {
                        return true;
                    }
                    if ((bissexto != 1) && (dia <= 28)) {
                        return true;
                    }
                    break;
            }
        }
    }
    return false;
}

/*
 * Método responsável por verificar se um objeto javascript possui valor:
 */
function hasValue(valor) {
    hasValue(valor, false);
}

/*
 * Método responsável por verificar se um objeto javascript possui valor.
 * valor: parametro a ser verificado se possui valor
 * str: parametro que indica se deve considerar string como valores vazios
 */
function hasValue(valor, str) {
    if ((!str && valor != null && valor != '' && (valor + '') != 'NaN' && valor != undefined && valor != 'null')
            || (str && valor != null && valor != undefined))
        return true;
    else
        return false;
}

/*
 * Método responsável por remover os espaços em branco de uma string:
 */
function trim(str) {
    return str.replace(/^\s+|\s+$/g, "");
}

/*
 * Método responsável por substituir várias ocorrências de strings:
 */
String.prototype.replaceAll = String.prototype.replaceAll || function (needle, replacement) {
    return this.split(needle).join(replacement);
};

/*
 * Função que exibe/oculta informações na tela
 */
function show(text) {
    if (document.getElementById(text).style.display == "none") {
        document.getElementById(text).style.display = "";
    } else {
        document.getElementById(text).style.display = "none";
    }
}

function showP(text, visible) {
    if (!visible)
        document.getElementById(text).style.display = "none";
    else
        document.getElementById(text).style.display = "";
}

function MensagensWeb(tipoMensagem, mensagem) {
    this.mensagem = mensagem;
    this.tipoMensagem = tipoMensagem;
}

var MENSAGEM_INFORMACAO = 3;
var MENSAGEM_AVISO = 2;
var MENSAGEM_ERRO = 1;
// Função responsável por apresentar as mensagens de informação, erro e warning para o usuário em tela:
function apresentaMensagem(idApresentador, error, mantem_mensagem) {
    var mensagensWeb = '';
    var apresentadorMensagem = document.getElementById(idApresentador);
    var div = '';
    var data = (hasValue(error) && hasValue(error.response)) ? error.response.data : '';

    try {
        error = eval(error);
        data = eval(error.response.data);
    }
    catch (ex) {
        //Não tem tratamento de mensagens, pois será tratado com JQuery logo abaixo.
    }
    if (hasValue(apresentadorMensagem)) {
        apresentadorMensagem.style.display = "none";
        apresentadorMensagem.style.height = "";

        if (!mantem_mensagem)
            apresentadorMensagem.mensagensAnteriores = null;

        if (hasValue(error) && hasValue(error.response)) { //aconteceu um erro
            if (hasValue(data) && hasValue(data.MensagensWeb))
                mensagensWeb = data.MensagensWeb;
            else if (hasValue(data)) {
                data = jQuery.parseJSON(data);

                if (!hasValue(data.MensagensWeb)) {
                    if (hasValue(data.erro) && hasValue(data.erro.MensagensWeb)) // ocorreu um erro no mvc.
                        mensagensWeb = data.erro.MensagensWeb;
                    else
                        mensagensWeb = jQuery.parseJSON(data).MensagensWeb;
                }
                else
                    mensagensWeb = data.MensagensWeb;
            }
        }
        else if (hasValue(error) && IsJsonString(error)
            && (hasValue(jQuery.parseJSON(error)) && hasValue(jQuery.parseJSON(error).MensagensWeb)))// retornou uma msg qualquer
                mensagensWeb = jQuery.parseJSON(error).MensagensWeb;
        else
            if (hasValue(error) && IsJsonString(error) &&
                 (hasValue(jQuery.parseJSON(error.responseText)) && hasValue(jQuery.parseJSON(error.responseText).erro.MensagensWeb)))// retornou uma msg qualquer
                mensagensWeb = jQuery.parseJSON(error.responseText).erro.MensagensWeb;
        else if (hasValue(error) && hasValue(error) && hasValue(error.MensagensWeb))// retornou uma msg qualquer no mvc
            mensagensWeb = error.MensagensWeb;
        else if (hasValue(error) && hasValue(error[0]) && hasValue(error[0].tipoMensagem))// criou a mensagem no js da própria página web
            mensagensWeb = error;
        else if (hasValue(error) && hasValue(error.erro) && hasValue(error.erro.MensagensWeb)) // ocorreu um erro no mvc.
            mensagensWeb = error.erro.MensagensWeb;

        if ((hasValue(mensagensWeb) && mensagensWeb.length > 0) || mantem_mensagem) {
            if (mantem_mensagem && hasValue(apresentadorMensagem.mensagensAnteriores) && hasValue(mensagensWeb))
                mensagensWeb = apresentadorMensagem.mensagensAnteriores.concat(mensagensWeb);
            else if (mantem_mensagem && hasValue(apresentadorMensagem.mensagensAnteriores) && !hasValue(mensagensWeb))
                mensagensWeb = apresentadorMensagem.mensagensAnteriores;

            if (hasValue(mensagensWeb) && mensagensWeb.length > 0) {
                apresentadorMensagem.mensagensAnteriores = mensagensWeb;
                apresentadorMensagem.style.display = "";

                if (mensagensWeb.length > 3)
                    apresentadorMensagem.style.height = "90px";

                div += '<table width="100%" class="tituloMensagens">';
                div += '    <tr>';
                div += '        <td style="width:185px">';
                div += '            Lista das mensagens ocorridas';
                div += '        </td>';
                div += '        <td><div style="line-height: 11px; border-bottom: 1px solid #dbdbdb;"/></td>';
                div += '        <td align="right" style="width:15px">';
                div += '            <a href="#" onclick="show(\'tb_mensagem_' + idApresentador + '\');" id="listaMensagens"><img  src="' + Endereco() + '/Content/themes/base/dojo1.8.5r/dojox/gantt/resources/images/collapse.png" id="imgDtx" style="width:12px" /><img  src="' + Endereco() + '/Content/themes/base/dojo1.8.5r/dojox/gantt/resources/images/expand.png" id="imgAtx" style="display:none; "width:12px"/></a>';
                div += '        </td>';
                div += '    </tr>';
                div += '</table>';

                div += '<div class="mensagensDiv" id="tb_mensagem_' + idApresentador + '">';
                div += '<table class="mensagens" width="100%">';
                for (i = 0; i < mensagensWeb.length; i++) {
                    div += '<TR>';
                    div += '    <TD style="width:4%; max-width:25px">';
                    if (mensagensWeb[i].tipoMensagem == MENSAGEM_INFORMACAO) {
                        div += '        <img  src="' + Endereco() + '/images/informacaoMsg.gif"/>';
                        div += '    </TD>';
                        div += '    <TD class="tbErro1">';
                        div += '        <div>' + mensagensWeb[i].mensagem + '</div>';
                        div += '    </TD>';
                    }
                    else if (mensagensWeb[i].tipoMensagem == MENSAGEM_AVISO) {
                        div += '        <img  src="' + Endereco() + '/images/avisoMsg.gif"/>';
                        div += '    </TD>';
                        div += '    <TD class="tbErro1">';
                        div += '        <div>' + mensagensWeb[i].mensagem + '</div>';
                        div += '    </TD>';
                    }
                    else {
                        apresentadorMensagem.style.height = "auto";
                        apresentadorMensagem.style.minHeight = "15px";
                        div += '        <img  src="' + Endereco() + '/images/erroMsg.gif"/>';

                        div += '    </TD>';
                        div += '    <TD class="tbErro1">';
                        if (hasValue(mensagensWeb[i].stack))
                            div += '        <div class="erro"><a href="#" onclick="show(\'div_error' + i + idApresentador + '\')">' + mensagensWeb[i].mensagem + '</a></div>';
                        else
                            div += '        <div class="erro">' + mensagensWeb[i].mensagem + '</div>';
                        div += '    </TD>';
                        div += '</TR>';
                        div += '<TR class="mensagens" id="div_error' + i + idApresentador + '" style="display:none">';
                        div += '    <TD colspan="2" class="tbErro2">';
                        if (hasValue(mensagensWeb[i].stack))
                            div += '            <div class="erro">' + mensagensWeb[i].stack + '</div>';
                        div += '    </TD>';
                    }
                    div += '</TR>';
                }
                div += '</table>';
                div += '</div>';

                div += '<table width="100%" class="tituloMensagens">';
                div += '    <tr>';
                div += '        <td><div style="line-height: 11px; border-bottom: 1px solid #dbdbdb; margin-top:-5px;">&nbsp;</td>';
                div += '    </tr>';
                div += '</table>';
                apresentadorMensagem.innerHTML = div;

                $(document).ready(function () {
                    //this.style.color
                    $("#listaMensagens").toggle(
                       function () {
                           $("#imgDtx").css("display", "none");
                           $("#imgAtx").css({ 'display': "", "width": "12px" });
                       },
                       function () {
                           $("#imgAtx").css('display', "none");
                           $("#imgDtx").css({ 'display': "", "width": "12px" });
                       });
                })
            }
        }
    }
}

// Verifica se parâmetro é um formato Json.
function IsJsonString(str) {
    try {
        jQuery.parseJSON(str);
    } catch (e) {
        return false;
    }
    return true;
}

// *** Função que alterar as  divs de alteração e Inclução fazendo com que as mesmas possam  desparecer ou aparecer conforme a ação do usuário. ***\\
function IncluirAlterar(valor, divAlterar, divIncluir, divExcluir, msg, divCancelar, divLimpar) {
    apresentaMensagem(msg, null);
    if (valor == 1) {
        document.getElementById(divIncluir).style.display = "";
        document.getElementById(divLimpar).style.display = "";
        document.getElementById(divCancelar).style.display = "none";
        document.getElementById(divAlterar).style.display = "none";
        if (divExcluir != null && divExcluir != undefined) {
            document.getElementById(divExcluir).style.display = "none";
        }
        
    }
    else {
        document.getElementById(divAlterar).style.display = "";
        document.getElementById(divCancelar).style.display = "";
        if (divExcluir != null && divExcluir != undefined) {
            document.getElementById(divExcluir).style.display = "";
        }
        document.getElementById(divLimpar).style.display = "none";
        document.getElementById(divIncluir).style.display = "none";
    }
}
//*** fim da função ***\\

//** Limpa os dados da pagina **\\
function getLimpar(form) {
    $(form).get(0).reset();
}

function caixaDialogo(ptipoDiag, pmsg, pFuncaoRetornoConfirmacao, pFuncaoRetornoCancelar) {
    require(["dojo/ready", "dojo/DialogConfirm", "dojo/_base/Deferred"], function (ready, DialogConfirm) {
        ready(function () {
            var Imagem = '';

            if (ptipoDiag == DIALOGO_AVISO)
                Imagem = '<img  src="' + Endereco() + '/images/avisoMsg.gif" style="padding-right: 10px;"/>';
            if (ptipoDiag == DIALOGO_INFORMACAO)
                Imagem = '<img  src="' + Endereco() + '/images/informacaoMsg.gif" style="padding-right: 10px;"/>';
            if (ptipoDiag == DIALOGO_ERRO)
                Imagem = '<img  src="' + Endereco() + '/images/erroMsg.gif" style="padding-right: 10px;"/>';
            pmsg = pmsg == '' ? 'Deseja remover esse(s) registro(s)?' : pmsg;
            var dialog = new DialogConfirm({
                title: (ptipoDiag == DIALOGO_CONFIRMAR) ? 'Confirmar' : (ptipoDiag == DIALOGO_AVISO) ? "Aviso" : (ptipoDiag == DIALOGO_INFORMACAO) ? "Informa\u00e7\u00e3o" : "Erro",
                hasSkipCheckBox: false,
                hasOkButton: (ptipoDiag == DIALOGO_CONFIRMAR),
                labelOkbutton: (ptipoDiag == DIALOGO_CONFIRMAR) ? 'Sim' : 'OK',
                labelCancelbutton: (ptipoDiag == DIALOGO_CONFIRMAR) ? 'N\u00e3o' : 'OK',
                content: (ptipoDiag == DIALOGO_CONFIRMAR) ? '<p>' + pmsg + '</p>' : '<p>' + Imagem + pmsg + '</p>'
            });
            dialog.show().then(function (remember) {
                // user pressed ok button
                // remember is true, when user wants you to remember his decision (user ticked the check box)
                if (!remember)
                    if (ptipoDiag == DIALOGO_CONFIRMAR && hasValue(pFuncaoRetornoConfirmacao, false))
                        pFuncaoRetornoConfirmacao.call();
            }, function (event) {
                if (ptipoDiag == DIALOGO_INFORMACAO && hasValue(pFuncaoRetornoConfirmacao, false))
                    pFuncaoRetornoConfirmacao.call();
                if (ptipoDiag == DIALOGO_CONFIRMAR && hasValue(pFuncaoRetornoCancelar, false))
                    pFuncaoRetornoCancelar.call();
                return false;
            })
        })
    })
}

// ** função que monta o comboBox de staus
function montarStatus(nomElement, simples) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try {
             var statusStore = null;

             if (simples)
                 statusStore = new Memory({
                     data: [
                         { name: "Ativos", id: "1" },
                         { name: "Inativos", id: "2" }
                     ]
                 });
             else
                 statusStore = new Memory({
                     data: [
                         { name: "Todos", id: "0" },
                         { name: "Ativos", id: "1" },
                         { name: "Inativos", id: "2" }
                     ]
                 });
             var status = new filteringSelect({
                 id: nomElement,
                 name: "status",
                 value: "1",
                 store: statusStore,
                 searchAttr: "name",
                 style: "width: 75px;"
             }, nomElement);
         }
         catch (e) {
             postGerarLog(e);
         }
     });
};

function montarStatusSingular(nomElement, simples) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         var statusStore = null;

         if (simples)
             statusStore = new Memory({
                 data: [
                     { name: "Ativo", id: "1" },
                     { name: "Inativo", id: "2" }
                 ]
             });
         else
             statusStore = new Memory({
                 data: [
                     { name: "Todo", id: "0" },
                     { name: "Ativo", id: "1" },
                     { name: "Inativo", id: "2" }
                 ]
             });
         var status = new filteringSelect({
             id: nomElement,
             name: "status",
             value: "1",
             store: statusStore,
             searchAttr: "name",
             style: "width: 75px;"
         }, nomElement);
     });
};

/*
 * Está Função server para criar o componente Select passando como parametro o elemento,os objetos de Dados, e o tamanho do compo em width. ex: width:100px
 */
function criarOuCarregarCompSelect(nomElement, data, tamanho, value, ready, Memory, Select, no_codigo, no_nome, FiltroSexo) {
    storeData = null;
    storeData = [];
    if (hasValue(FiltroSexo)) {
        /*
        if (FiltroSexo == MASCULINO)
            storeData.push({ id: 0, name: "Todos" });
        else
            storeData.push({ id: 0, name: "Todas" });*/
        storeData.push({ id: 0, name: "Todas" });
        value = 0;
    }

    if (hasValue(data) && data.length > 0) {

        $.each(data, function (index, value) {
            storeData.push({
                id: eval("value." + no_codigo),
                label: eval("value." + no_nome)
            });
        });
    }

    var memory = new Memory({
        data: storeData
    });
    var statusStore = new dojo.data.ObjectStore({ objectStore: memory });

    if (!hasValue(dijit.byId(nomElement), true))
        var status = new Select({
            id: nomElement,
            name: nomElement,
            store: statusStore,
            searchAttr: "name",
            value: value,
            style: tamanho
        }, nomElement);
    else
        dijit.byId(nomElement).setStore(statusStore);

    if (value != null && hasValue(value, true))
        dijit.byId(nomElement).set("value", value);
}

/*
 * Está Função server para criar o componente filteringSelect passando como parametro o elemento,os objetos de Dados, e o tamanho do compo em width. ex: width:100px
 */
function criarOuCarregarCompFiltering(nomElement, data, tamanho, value, ready, Memory, filteringSelect, no_codigo, no_nome, FiltroSexo) {
    storeData = null;
    storeData = [];
    if (hasValue(FiltroSexo)) {
        /*
        if (FiltroSexo == MASCULINO)
            storeData.push({ id: 0, name: "Todos" });
        else
            storeData.push({ id: 0, name: "Todas" });*/
        storeData.push({ id: 0, name: "Todas" });
        value = 0;
    }

    if (hasValue(data) && data.length > 0) {

        $.each(data, function (index, value) {
            storeData.push({
                id: eval("value." + no_codigo),
                name: hasValue(eval("value." + no_nome)) ? eval("value." + no_nome) : ""
            });
        });
    }

    var statusStore = new Memory({
        data: storeData
    });
    var dijitElement = dijit.byId(nomElement);
    if (!hasValue(dijitElement, true))
        var status = new filteringSelect({
            id: nomElement,
            name: nomElement,
            store: statusStore,
            searchAttr: "name",
            value: value,
            style: tamanho
        }, nomElement);
    else
        dijitElement.store = statusStore;

    if (hasValue(dijitElement, true) && value != null && hasValue(value, true))
        dijitElement.set("value", value);
    return dijitElement;
}

function criarOuCarregarCompFilteringChange(nomElement, data, tamanho, value, ready, Memory, filteringSelect, no_codigo, no_nome, FiltroSexo) {
    storeData = null;
    storeData = [];
    if (hasValue(FiltroSexo)) {
        storeData.push({ id: 0, name: "Todas" });
        value = 0;
    }

    if (hasValue(data) && data.length > 0) {

        $.each(data, function (index, value) {
            storeData.push({
                id: eval("value." + no_codigo),
                name: hasValue(eval("value." + no_nome)) ? eval("value." + no_nome) : ""
            });
        });
    }

    var statusStore = new Memory({
        data: storeData
    });
    var dijitElement = dijit.byId(nomElement);
    if (!hasValue(dijitElement, true))
        var status = new filteringSelect({
            id: nomElement,
            name: nomElement,
            store: statusStore,
            searchAttr: "name",
            value: value,
            style: tamanho
        }, nomElement);
    else
        dijitElement.store = statusStore;

    if (hasValue(dijitElement, true) && value != null && hasValue(value, true)) {
        dijitElement._onChangeActive = false;
        dijitElement.set("value", value);
        dijitElement._onChangeActive = true;
    }
    return dijitElement;
}

function criarOuCarregarCompFilteringBanco(nomElement, data, tamanho, value, ready, Memory, filteringSelect, no_codigo, no_nome, FiltroSexo) {
    storeData = null;
    storeData = [];
    if (hasValue(FiltroSexo)) {
        /*if (FiltroSexo == MASCULINO)
            storeData.push({ id: 0, name: "Todos" });
        else
            storeData.push({ id: 0, name: "Todas" });*/
        storeData.push({ id: 0, name: "Todas" });
        value = 0;
    }

    if (hasValue(data) && data.length > 0) {

        $.each(data, function (index, value) {
            storeData.push({
                id: eval("value." + no_codigo),
                name: hasValue(eval("value." + no_nome)) ? eval("value." + no_nome) : "",
                nm_banco: hasValue(eval("value." + "localMovtoCateiraCnab.nm_banco")) ? eval("value." + "localMovtoCateiraCnab.nm_banco") : ""
            });
        });
    }

    var statusStore = new Memory({
        data: storeData
    });
    var dijitElement = dijit.byId(nomElement);
    if (!hasValue(dijitElement, true))
        var status = new filteringSelect({
            id: nomElement,
            name: nomElement,
            store: statusStore,
            searchAttr: "name",
            value: value,
            style: tamanho
        }, nomElement);
    else
        dijitElement.store = statusStore;

    if (hasValue(dijitElement, true) && value != null && hasValue(value, true))
        dijitElement.set("value", value);
    return dijitElement;
}

function criarOuCarregarCompFilteringNotasMat(nomElement, data, tamanho, value, ready, Memory, filteringSelect, no_codigo, no_nome, cd_curso, id_venda_futura) {
    storeData = null;
    storeData = [];

    if (hasValue(data) && data.length > 0) {

        $.each(data, function (index, value) {
            storeData.push({
                id: eval("value." + no_codigo),
                name: hasValue(eval("value." + no_nome)) ? eval("value." + no_nome) : "",
                cd_curso: hasValue(eval("value." + cd_curso)) ? eval("value." + cd_curso) : "",
                id_venda_futura: hasValue(eval("value." + id_venda_futura)) ? eval("value." + id_venda_futura) : ""
            });
        });
    }

    var statusStore = new Memory({
        data: storeData
    });
    var dijitElement = dijit.byId(nomElement);
    if (!hasValue(dijitElement, true))
        var status = new filteringSelect({
            id: nomElement,
            name: nomElement,
            store: statusStore,
            searchAttr: "name",
            value: value,
            style: tamanho
        }, nomElement);
    else
        dijitElement.store = statusStore;

    if (hasValue(dijitElement, true) && value != null && hasValue(value, true))
        dijitElement.set("value", value);
    return dijitElement;
}

/*
 * Função que verifica se o usuario possui determinada permissão na tela. Usada para desabilitar componentes visuais.
 */
function possuiPermissao(permissao, permissoes) {
    var arrayPermissoes = permissoes.split('|');
    var retorno = false;

    for (var i = 0; i < arrayPermissoes.length && !retorno; i++)
        if (arrayPermissoes[i] == permissao)
            retorno = true;

    return retorno;
}

//Função que limpa o Form
function clearForm(form) {
    $(".tituloMensagens").hide();
    $(".mensagens").hide();
    require(["dojo/dom"], function (dom) {
        var f = dojo.byId(form);
        if (!hasValue(f))
            f = dijit.byId(form);
        if (hasValue(f) && hasValue(f.elements)) // Limpa os elementos de um formulário:
            for (var i = 0; i < f.elements.length; i++) {
                var elem = f.elements[i];
                var dijitElem = dijit.byId(elem.id);
                if (hasValue(dijitElem)) {
                    dijitElem._onChangeActive = false;
                    dijitElem.reset();
                    dijitElem._onChangeActive = true;
                }
            }
    });
}

//Função que desabilita o FilteringSelect, so precisa passar o componente para  função. exe: toggleDisabled(dijit.byId("!"))
function toggleDisabled(widget, bool) {
    widget = dijit.byId(widget);
    widget.set('disabled', bool);
}

//Retorna o valor do ComboBox
function retornaStatus(status) {
    var statusDijit = dijit.byId(status);
    var tipo = 0;

    if (hasValue(statusDijit))
        tipo = statusDijit.get("value");
    return parseInt(tipo);
}

function loadSelect(items, link, idName, valueName, id) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    var itemsCb = [];
		    Array.forEach(items, function (value, i) {
		        itemsCb.push({ id: eval("value." + idName), name: eval("value." + valueName) });
		    });
		    var stateStore = new Memory({
		        data: itemsCb
		    });
		    var componente = dijit.byId(link);

		    componente._onChangeActive = false;
		    componente.store = stateStore;
		    if (hasValue(id))
		        componente.set("value", id);
		    componente._onChangeActive = true;
		})
}

function loadMultiSelect(items, link, idName, valueName, marcaTodos) {

    require([
        "dojo/ready",
        "dojo/data/ItemFileReadStore",
        "dojo/_base/array"
    ], function (ready, ItemFileReadStore, BaseArray) {
        ready(function () {
            var itemsCb = [];
            var itensMarcados = new Array();

            BaseArray.forEach(items, function (value, i) {
                itemsCb.push({ 'id': eval("value." + idName) + "", 'name': eval("value." + valueName) });
                itensMarcados[i] = eval("value." + idName);
            });
            var store = new ItemFileReadStore({
                data: {
                    identifier: 'id',
                    label: 'name',
                    items: itemsCb
                }
            });

            var componente = dijit.byId(link);

            componente.setStore(store, []);

            if (marcaTodos) {
                componente._onChangeActive = false;
                componente.setStore(componente.store, itensMarcados);
                componente._onChangeActive = true;
            }
        });
    });
}

//*********************************************************************************************************
//	Função para abrir o popup:
//*********************************************************************************************************
var janela = null;
function abrePopUp(caminho_pagina, tam_horizontal, tam_vertical, nome) {
    if (caminho_pagina.indexOf('manual') > 0) {

        if (!hasValue(nome))
            nome = 'popup';

        if (hasValue(janela) && !janela.closed && janela.name == nome) {
            janela.close();
            janela = null;
        }

        janela = window.open(caminho_pagina, nome, 'width=' + tam_horizontal + ', height=' + tam_vertical + ', scrollbars=yes, menubar=no, resizable=yes, status=no, titlebar=no, toolbar=no,top=100,left=50');
    }
    else
        janela = window.open(caminho_pagina, nome + Math.floor(Math.random() * 16777215).toString(16));
    if (hasValue(janela))
        janela.window.moveTo(125, 100);
    setTimeout(function () { janela.focus(); }, 10);
}

function abrePopUpChat() {
    abreIntegracao("/Integracao/getUrlChat");
}

function abrePopUpURLAreaRestrita() {
    abreIntegracao("/Integracao/getUrlAreaRestrita");
}

function abrePopUpURLAreaRestritah() {
    abreIntegracao("/Integracao/getUrlAreaRestritah");
}

function abrePopUpURLECommerce() {
    abreIntegracao("/Integracao/getUrlEcommerce");
}

function abrePopUpURLPortal(cd_aluno) {
    abreIntegracao("/Integracao/getUrlPortal?cd_aluno="+cd_aluno);
}

function abrePopUpURLDashboard() {
    abreIntegracao("/Integracao/getUrlDashboard");
}

function abreIntegracao(caminho) {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + caminho,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            if (hasValue(data.erro) && hasValue(data.erro.MensagensWeb) && data.erro.MensagensWeb.length > 0) {
                caixaDialogo(DIALOGO_ERRO, data.erro.MensagensWeb[0].stack, null);
            } else {
                if (navigator.vendor != null &&
                    navigator.vendor.match(/Apple Computer, Inc./) &&
                    navigator.userAgent.indexOf('Safari') != -1) {
                    window.location.assign(data.retorno);
                } else {
                    janela = window.open(data.retorno, 'popupECommerce' + Math.floor(Math.random() * 16777215).toString(16));
                }

            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

function abreMenu(dc_url_menu) {
    if (dc_url_menu.indexOf('abrePopUpURL') == -1)
        window.location = dc_url_menu;
    else
        eval(dc_url_menu);
}

//*********************************************************************************************************
//	Função para abrir o popup:
//*********************************************************************************************************
function abrePopUpSemResizable(caminho_pagina, tam_horizontal, tam_vertical, nome) {
    janela = window.open(caminho_pagina, nome, 'width=' + tam_horizontal + ', height=' + tam_vertical + ', scrollbars=yes, menubar=no, resizable=no, status=no, titlebar=no, toolbar=no,top=100,left=50');
    janela.window.moveTo(125, 100);
    janela.focus();
}


//**** Função regular Verifica se é um inteiro válido e retorna mensagem para o usuário se não for
function isNumeric(str, mensagem, apresentador) {
    var er = /^[0-9]+$/;
    var mensagensWeb = new Array();
    if (hasValue(apresentador) && (er.test(str)) == false) {
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, mensagem);
        apresentaMensagem(apresentador, mensagensWeb);
    };
    return (er.test(str));
}

//=====================================================================================
//	Função para tratar campos do tipo numéricos float. Só permite caracteres de 0 a 9, backsapce, delete, seta para os lados, virgula  e enter.
//  Exemplo de uso no dojo: mascaraFloat(document.getElementById('pc_inicial_conceito'));
//=====================================================================================
function mascaraFloat(objCpo) {
    if ((!((event.keyCode >= 48) &&
        (event.keyCode <= 57))) &&
        (event.keyCode != 13) &&
        (event.keyCode != 44) && (event.keyCode != 8) && (event.keyCode != 9) &&
        (event.keyCode != 37) && (event.keyCode != 39) && (event.keyCode != 46)) {
        event.returnValue = false;
        return false;
    }

    var vlrCpo = objCpo.value.split(",");
    if ((event.keyCode == 44) && (vlrCpo.length > 1)) {
        event.returnValue = false;
        return false;
    }
    return true;
}

//=====================================================================================
//	Função para tratar campos do tipo numéricos float. Só permite caracteres de 0 a 9, backsapce, delete, seta para os lados, virgula  e enter.
//  Exemplo de uso no dojo: mascaraFloat(document.getElementById('pc_inicial_conceito'));
//=====================================================================================
function mascaraFloat(objCpo, casas) {
    if ((!((event.keyCode >= 48) &&
        (event.keyCode <= 57))) &&
        (event.keyCode != 13) &&
        (event.keyCode != 44) && (event.keyCode != 8) && (event.keyCode != 9) &&
        (event.keyCode != 37) && (event.keyCode != 39) && (event.keyCode != 46)) {
        event.returnValue = false;
        return false;
    }

    var vlrCpo = objCpo.value.split(",");
    if (event.keyCode != 46 && event.keyCode != 8 && ((event.keyCode == 44 && vlrCpo.length > 1) || (vlrCpo.length > 1 && vlrCpo[1].length + 1 > casas))) {
        event.returnValue = false;
        return false;
    }
    return true;
}
//=====================================================================================
//	Função para tratar campos do tipo numéricos inteiros. Só permite caracteres de 0 a 9, backsapce, delete, seta para os lados  e enter.
//  Exemplo de uso no dojo: mascaraInt(document.getElementById('nm_ordem_estagio'));
//=====================================================================================
function mascaraInt(objCpo) {
    if ((!((event.keyCode >= 48) &&
        (event.keyCode <= 57))) &&
        (event.keyCode != 13) &&
        (event.keyCode != 8) &&
        (event.keyCode != 9) &&
        (event.keyCode != 37) &&
        (event.keyCode != 39)) {
        event.returnValue = false;
        return false;
    }
    return true;
}

//=====================================================================================
//	Função para tratar campos do tipo numéricos inteiros. Só permite caracteres de 0 a 9, backsapce, delete, seta para os lados  e enter.
//  Exemplo de uso no dojo: mascaraTelefone(document.getElementById('nm_ordem_estagio'));
//=====================================================================================
function mascaraTelefone(objCpo) {
    if ((!((event.keyCode >= 48) &&
        (event.keyCode <= 57))) &&
        (event.keyCode != 13) &&
        (event.keyCode != 8) &&
        (event.keyCode != 9) &&
        (event.keyCode != 37) &&
        (event.keyCode != 32) &&
        (event.keyCode != 40) &&
        (event.keyCode != 45) &&
        (event.keyCode != 41) &&
        (event.keyCode != 39)) {
        event.returnValue = false;
        return false;
    }
    return true;
}

function mascaraIntNegativo(objCpo) {
    if ((!((event.keyCode >= 48) &&
        (event.keyCode <= 57))) &&
        (event.keyCode != 13) &&
        (event.keyCode != 8) &&
        (event.keyCode != 9) &&
        (event.keyCode != 37) &&
        (event.keyCode != 39) &&
        (event.keyCode != 45)) {
        event.returnValue = false;
        return false;
    }
    return true;
}

//============================================================================
// Funções para pegar Variáveis Globais que serão usadas em todos os módúlos
function Endereco() {
    return document.getElementById("_ER0").value;
}
function EnderecoAbsoluto() {
    return document.getElementById("_ER1").value;
}
function EnderecoRelatorioWeb() {
    return document.getElementById("_ERW").value;
}
function Escola() {
    return document.getElementById("_ES0").value;
}
function Token() {
    return document.getElementById("_TK0").value;
}
function EscolasUsuario() {
    return document.getElementById("_EU0").value;
}
function Master() {
    return document.getElementById("_IM0").value;
}
function MasterGeral() {
    return document.getElementById("_IM1").value;
}
function LoginUsuario() {
    return document.getElementById("_LU0").value;
}
function FechConsolidado() {
    return document.getElementById("_FCC").value;
}


//=============================================================================

//============================================================================
// Funções para manipular em uma lista js de forma perfomática usando quicksort - log(n):
Array.prototype.swap = function (a, b) {
    var tmp = this[a];
    this[a] = this[b];
    this[b] = tmp;
}

function calculaMediaAritmeticaLog(array, soma, qtd) {
    return calculaSomaLog(array, soma) / qtd;
}

function calculaSomaLog(array, soma) {
    if (array.length == 0)
        return soma;
    else if (array.length == 1)
        return soma + array[0];
    else if (array.length > 1) {
        var pivot = Math.floor(array.length / 2);
        var partition = array.splice(0, pivot);
        return soma + calculaSomaLog(partition, soma) + calculaSomaLog(array, soma);
    }
}

function partition(array, begin, end, pivot) {
    var piv = array[pivot];
    array.swap(pivot, end - 1);
    var store = begin;
    var ix;

    for (ix = begin; ix < end - 1; ++ix) {
        var arrayAux = parseFloat(array[ix]);
        var pivAux = parseFloat(piv);

        if (arrayAux + '' != 'NaN')
            array[ix] = arrayAux;
        if (pivAux + '' != 'NaN')
            piv = pivAux;

        if (array[ix] <= piv) {
            array.swap(store, ix);
            ++store;
        }
    }
    array.swap(end - 1, store);

    return store;
}

function partitionObj(array, campo, begin, end, pivot) {
    var piv = eval('array[pivot].' + campo);
    array.swap(pivot, end - 1);
    var store = begin;
    var ix;

    for (ix = begin; ix < end - 1; ++ix) {
        var propriedade = eval('array[ix].' + campo);
        var propriedadeAux = parseFloat(propriedade);
        var pivAux = parseFloat(piv);

        if (propriedadeAux + '' != 'NaN')
            propriedade = propriedadeAux;
        if (pivAux + '' != 'NaN')
            piv = pivAux;

        if (propriedade <= piv) {
            array.swap(store, ix);
            ++store;
        }
    }
    array.swap(end - 1, store);

    return store;
}

function qsort(array, begin, end) {
    if (end - 1 > begin) {
        var pivot = begin + Math.floor(Math.random() * (end - begin));
        //var pivot = begin + Math.floor((end - begin)/2);

        pivot = partition(array, begin, end, pivot);
        qsort(array, begin, pivot);
        qsort(array, pivot + 1, end);
    }
}

function qsortObj(array, campo, begin, end) {
    if (end - 1 > begin) {
        var pivot = begin + Math.floor(Math.random() * (end - begin));
        //var pivot = begin + Math.floor((end - begin)/2);

        pivot = partitionObj(array, campo, begin, end, pivot);
        qsortObj(array, campo, begin, pivot);
        qsortObj(array, campo, pivot + 1, end);
    }
}

function quickSort(array) {
    qsort(array, 0, array.length);
}

function quickSortObj(array, campo) {
    qsortObj(array, campo, 0, array.length);
}

// Função para excluir da lista formatada por ponto e virgula:
function removeStrSort(str, value) {
    value = value + "";
    if (hasValue(str))
        if (str == value)
            return new Array(); //Lista de um único item e o item é o valor procurado
        else {
            var novaLista = new Array();

            if (str.indexOf(value + ";") == 0)
                novaLista = str.substring(value.length + 1);
            else if (str.indexOf(value) == (str.length - value.length))
                novaLista = str.substring(0, str.length - value.length - 1); //Valor procurado é o último da lista
            else
                novaLista = str.replace(';' + value + ';', ';'); //Valor procurado está no meio lista

            if (novaLista.indexOf(";") < 0) {
                if (!hasValue(novaLista))
                    return new Array();
                else {
                    var array = new Array();
                    array.push(novaLista)
                    return array;
                }
            }
            return novaLista.split(';'); //Valor procurado é o primeiro da lista
        }
}

// Função para excluir da lista formatada:
function removeSort(array, value) {
    if (hasValue(array) && array.length > 0) {
        var str = array.join(';');
        return removeStrSort(str, value);
    }
    return array;
}

function removeObjSort(array, campo, value) {
    if (hasValue(array))
        return removeObjSortAux(array, campo, value, 0, array.length - 1);
}

function removeObjSortAux(array, campo, value, inicio, fim) {
    var pivot = inicio + Math.floor((fim - inicio) / 2);
    var valor = null;

    if (hasValue(array) && array.length > 0)
        valor = eval('array[pivot].' + campo);

    if (hasValue(valor)) {
        if (valor == value)
            return array.splice(pivot, 1);
        else if (valor > value && pivot - 1 >= 0)
            removeObjSortAux(array, campo, value, inicio, pivot - 1);
        else if (pivot + 1 <= array.length && inicio <= fim)
            removeObjSortAux(array, campo, value, pivot + 1, fim);
    }
}

// Função para incluir e retornar uma lista separada por ponto e virgula de forma ordenada:
function insertStrSort(str, value, duplicate) {
    var array = str.split(';');

    array = insertSort(array, value, duplicate);

    return array.join(';');
}

// Função para incluir e retornar uma lista de forma ordenada:
function insertSort(array, value, duplicate) {
    var position = array.length;

    if (!duplicate && !binarySearch(array, value)) {
        if (!hasValue(array) || array.length == 0)
            array[0] = value;
        else
            array[position] = value;
        quickSort(array);
    }

    return array;
}

// Função para incluir e retornar um objeto na lista de forma ordenada:
function insertObjSort(array, campo, value, duplicate) {
    var position = array.length;

    if (duplicate || (!duplicate && binaryObjSearch(array, campo, eval('value.' + campo)) == null)) {
        if (!hasValue(array) || array.length == 0)
            array[0] = value;
        else
            array[position] = value;
        quickSortObj(array, campo);
    }

    return array;
}

// Função que verifica se um elemento se encontra no array em tempo logarítimo:
function binarySearch(array, value) {
    return binarySearchAux(array, value, 0, array.length - 1);
}

// Função que retorna a posição do objeto no array ordenado se existir ou nulo quando não existir.
function binaryObjSearch(array, campo, value) {
    if (hasValue(array))
        return binaryObjSearchAux(array, campo, value, 0, array.length - 1);
    return null;
}

function binarySearchAux(array, value, inicio, fim) {
    var retorno = false;
    var pivot = inicio + Math.floor((fim - inicio) / 2);

    if (hasValue(array) && array.length > 0 && inicio >= 0 && fim < array.length) {
        if ((array.length == 1 && array[0] == value) || array[pivot] == value)
            retorno = true;
        else
            if (array[pivot] < value && pivot + 1 <= fim)
                return binarySearchAux(array, value, pivot + 1, fim);
            else if (pivot - 1 >= 0 && inicio <= fim)
                return binarySearchAux(array, value, inicio, pivot - 1);
    }
    return retorno;
}

function binaryObjSearchAux(array, campo, value, inicio, fim) {
    var retorno = null;
    var pivot = inicio + Math.floor((fim - inicio) / 2);

    if (inicio == fim && eval('array[pivot].' + campo) != value)
        return retorno;
    else
        if (hasValue(array) && array.length > 0 && inicio >= 0 && fim < array.length) {
            if ((array.length == 1 && eval('array[0].' + campo) == value) || eval('array[pivot].' + campo) == value)
                retorno = pivot;
            else
                if (eval('array[pivot].' + campo) < value && pivot + 1 <= fim)
                    return binaryObjSearchAux(array, campo, value, pivot + 1, fim);
                else if (pivot - 1 >= 0 && inicio <= fim)
                    return binaryObjSearchAux(array, campo, value, inicio, pivot - 1);
        }
    return retorno;
}


/**** Função para ordernar por várias propriedades ****/
Array.prototype.keySort = function (keys) {

    keys = keys || {};

    // via
    // http://stackoverflow.com/questions/5223/length-of-javascript-object-ie-associative-array
    var obLen = function (obj) {
        var size = 0, key;
        for (key in obj) {
            if (obj.hasOwnProperty(key))
                size++;
        }
        return size;
    };

    // avoiding using Object.keys because I guess did it have IE8 issues?
    // else var obIx = function(obj, ix){ return Object.keys(obj)[ix]; } or
    // whatever
    var obIx = function (obj, ix) {
        var size = 0, key;
        for (key in obj) {
            if (obj.hasOwnProperty(key)) {
                if (size == ix)
                    return key;
                size++;
            }
        }
        return false;
    };

    var keySort = function (a, b, d) {
        d = d !== null ? d : 1;
        // a = a.toLowerCase(); // this breaks numbers
        // b = b.toLowerCase();
        if (a == b)
            return 0;
        return a > b ? 1 * d : -1 * d;
    };

    var KL = obLen(keys);

    if (!KL)
        return this.sort(keySort);

    for (var k in keys) {
        // asc unless desc or skip
        keys[k] =
                keys[k] == 'desc' || keys[k] == -1 ? -1
              : (keys[k] == 'skip' || keys[k] === 0 ? 0
              : 1);
    }

    this.sort(function (a, b) {
        var sorted = 0, ix = 0;

        while (sorted === 0 && ix < KL) {
            var k = obIx(keys, ix);
            if (k) {
                var dir = keys[k];
                sorted = keySort(a[k], b[k], dir);
                ix++;
            }
        }
        return sorted;
    });
    return this;
};


/* Funções para componentização de checkbox na grid paginada sob demanda: */
function checkBoxChange(rowIndex, field, fieldTodos, idTodos, obj, grid) {
    var itemTodos = dijit.byId(idTodos);

    if (rowIndex != -1 && hasValue(grid.getItem(rowIndex))) {
        if (!hasValue(grid.itensSelecionados))
            grid.itensSelecionados = [];

        if (obj.checked) {
            insertObjSort(grid.itensSelecionados, field, grid.getItem(rowIndex));
            verificaMarcacaoTodos(function () { marcaItem(itemTodos, true); }, fieldTodos, grid);
        }
        else {
            removeObjSort(grid.itensSelecionados, field, eval('grid.getItem(rowIndex).' + field));
            marcaItem(itemTodos, false);
        }
    }
    else
        // Checa todos:
        selecionaTodos(fieldTodos, itemTodos.checked);
}

function verificaMarcacaoTodos(funcaoRetorno, fieldTodos, grid) {
    var j = 0;
    var campo = dojo.byId(fieldTodos + '_Selected_' + j);
    var value = true;
    var finalizouSemResposta = false;

    while (hasValue(campo) && value) {
        if (campo.type == 'text') {
            setTimeout("verificaMarcacaoTodos(" + funcaoRetorno + ", '" + fieldTodos + "', " + grid + "))", grid.rowsPerPage * 3);
            finalizouSemResposta = true;
            break;
        }
        else {
            value = value && campo.checked;
            j += 1;
            campo = dojo.byId(fieldTodos + '_Selected_' + j);
        }
    }
    if (!finalizouSemResposta && value)
        funcaoRetorno.call();
}

function configuraCheckBox(value, field, fieldTodos, rowIndex, id, idTodos, gridName, disabled) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            var dojoId = dojo.byId(id);
            var grid = dijit.byId(gridName);

            if (hasValue(grid)){
            if (id != idTodos || (hasValue(grid.pagination) && !grid.pagination.plugin._showAll)) {

                // Se id for seleciona todos, verifica se todos estão marcados para marcá-lo:
                if (id == idTodos) {
                    var j = 0;
                    var campo = dojo.byId(fieldTodos + '_Selected_' + j);
                    value = hasValue(campo);

                    while (hasValue(campo) && value) {
                        if (campo.type == 'text') {
                            setTimeout("configuraCheckBox(" + value + ", '" + field + "', '" + fieldTodos + "', " + rowIndex + ", '" + id + "', '" + idTodos + "', '" + gridName + "')", grid.rowsPerPage * 3);
                            return;
                        }
                        else {
                            value = value && campo.checked;
                            j += 1;
                            campo = dojo.byId(fieldTodos + '_Selected_' + j);
                        }
                    }
                }

                if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                    dijit.byId(id).destroy();
                if (value == undefined)
                    value = false;
                if (disabled == null || disabled == undefined) disabled = false;
                if (hasValue(dojoId) && dojoId.type == 'text')
                    var checkBox = new dijit.form.CheckBox({
                        name: "checkBox",
                        checked: value,
                        disabled: disabled,
                        onChange: function (b) { checkBoxChange(rowIndex, field, fieldTodos, idTodos, this, grid); }
                    }, id);
            }
            else if (hasValue(dojo.byId(idTodos)))
                dojo.byId(idTodos).parentNode.removeChild(dojo.byId(idTodos));

            }
        })
    });
}

function selecionaTodos(field, value) {
    var index = 0;
    var item = dijit.byId(field + '_Selected_' + index);

    while (hasValue(dojo.byId(field + '_Selected_' + index))) {
        //item._onChangeActive = false;
        item.set('value', value);
        //item._onChangeActive = true;

        item.checked = value;
        index += 1;
        item = dijit.byId(field + '_Selected_' + index);
    }
}

function marcaItem(item, value) {
    if (hasValue(item)) {
        item._onChangeActive = false;
        item.set('value', value);
        item._onChangeActive = true;
    }
}

function verificaMostrarTodos(evt, grid, campo, idTodos) {
    var marcarTodos = dijit.byId(idTodos);
    if (hasValue(marcarTodos) && hasValue(evt.srcElement) && hasValue(evt.srcElement.value) && evt.srcElement.value == 'Infinity')
        marcarTodos.destroy();
}


function setGridPagination(grid, item, campo) {
    // Busca pelo nome da coluna que a grid quando está ordanada:
    //if (hasValue(grid.sortInfo))
    //    campo = grid.layout.cells[grid.sortInfo-1].field;

    // Busca a posição do item no grid de forma perfomática:
    var posicao = binaryObjSearch(grid.store.objectStore.data, campo, eval('item.' + campo));

    if (posicao != null) {
        // Calcula a posição na grid:
        var posicaoPagination = Math.floor(posicao / grid._eop);

        // Posiciona a grid na posição do item:
        grid.currentPage(posicaoPagination + 1);
    }
}

function buscarItensSelecionados(gridName, fieldTodos, id, idTodos, camposDesabilitados, todosItensName) {
    var grid = dijit.byId(gridName);
    require([
          "dojo/data/ObjectStore",
          "dojo/store/Memory"
    ], function (ObjectStore, Memory) {
        var todos = dojo.byId(todosItensName + "_label");
        var dataStore = new ObjectStore({ objectStore: new Memory({ data: grid.itensSelecionados }) });

        if (hasValue(todos))
            todos.innerHTML = "Itens Selecionados";

        grid.setStore(dataStore);

        dataStore.fetch({
            onComplete: function (items, request) {
                // Configura o check de todos:
                var todos = dojo.byId(idTodos);
                if (hasValue(todos) && todos.type == 'text') {
                    setTimeout("configuraCheckBox(false, null, '" + fieldTodos + "', -1, '" + idTodos + "', '" + idTodos + "', '" + gridName + "')", grid.rowsPerPage * 3);
                }
            }
        });

        // Desabilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
        for (var i = 0; hasValue(camposDesabilitados) && i < camposDesabilitados.length; i++)
            dijit.byId(camposDesabilitados[i]).set('disabled', true);
    });
}

function buscarTodosItens(grid, todosItensName, camposHabilitados) {
    var todos = dojo.byId(todosItensName + "_label");

    if (hasValue(todos))
        todos.innerHTML = "Todos Itens";

    // Habilita os botões de pesquisar e relatório:
    for (var i = 0; hasValue(camposHabilitados) && i < camposHabilitados.length; i++)
        dijit.byId(camposHabilitados[i]).set('disabled', false);
}

function eventoRemover(itensSelecionados, funcao) {
    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
        caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
    else
        caixaDialogo(DIALOGO_CONFIRMAR, '', function () { eval(funcao) });
}
function btnPesquisar(btn) {

    if (hasValue(btn)) {
        btn.parentNode.style.minWidth = '18px';
        btn.parentNode.style.width = '18px';
    }
}

function decreaseBtn(btn, px) {
    if (hasValue(btn)) {
        btn.parentNode.style.minWidth = px;
        btn.parentNode.style.width = px;
    }
}
//Esse metodo recebe um array de IDs.
function diminuirBotoes(buttonFkArray) {
    for (var p = 0; p < buttonFkArray.length; p++) {
        var buttonFk = document.getElementById(buttonFkArray[p]);

        if (hasValue(buttonFk)) {
            buttonFk.parentNode.style.minWidth = '18px';
            buttonFk.parentNode.style.width = '18px';
        }
    }
}

//=============================================================================

function getParamterosURL() {
    // Pega os parametros da página:
    var params = {};

    if (location.search) {
        var parts = location.search.substring(1).split('&');

        for (var i = 0; i < parts.length; i++) {
            var nv = parts[i].split('=');
            if (!nv[0]) continue;
            params[nv[0]] = nv[1] || true;
        }
    }

    return params;
}

//Metodo para mostrar a mensagem s eo campo for requerid-true.
function mostrarMensagemCampoValidado(windowUtils, widget) {
    var isValid = true;
    if (widget != null && hasValue(windowUtils)) {
        // Need to set this so that "required" widgets get their
        // state set.
        widget._hasBeenBlurred = true;
        var valid = widget.disabled || !widget.validate || widget.validate();
        if (!valid) {
            // Set focus of the first non-valid widget
            if (typeof windowUtils.scrollIntoView !== "undefined") {
	            windowUtils.scrollIntoView(widget.containerNode || widget.domNode);
            }
            var widgetDijit = dijit.byId(widget.id);

            //if (!widget.didFocus)
            //    setTimeout(function () { widget.focus() }, 100);
            //widget.didFocus = true;
        }
        isValid = isValid && valid;
    }
    return isValid;
}

function deletarItemSelecionadoGrid(Memory, ObjectStore, nomeId, grid) {
    try {
        grid.store.save();
        var dados = grid.store.objectStore.data;

        if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {
            //Percorre a lista da grade para deleção (O(n)):
            for (var i = dados.length - 1; i >= 0; i--)
                // Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
                if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null)
                    dados.splice(i, 1); // Remove o item do array

            grid.itensSelecionados = new Array();
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
            grid.setStore(dataStore);
            grid.update();
        }
        else
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Limpar data/hora para não ficar com obrigatoriadade, isso devido a mask do jQurey

function limparDateHoraTextBox(obj) {
    var dojoObj = dojo.byId(obj.name);
    if (dojoObj.value == '__/__/____' || dojoObj.value == '__:__') {
        dojoObj.value = '';
        obj.validate();
    }
}

//Método usado para clonar objeto
function clone(obj) {
    if (obj == null || typeof (obj) != 'object')
        return obj;
    var temp = new obj.constructor();
    for (var key in obj)
        temp[key] = clone(obj[key]);
    return temp;
}

function cloneArray(array) {
    var retorno = new Array();

    if (hasValue(array) && array["length"] != undefined && array["length"] != null) {
	    for (var i = 0; i < array.length; i++)
		    retorno.push(clone(array[i]));

	    return retorno;
    }

    return retorno;

}
//fim

function getDiaSemana(id) {
    switch (id) {
        case 0: return 'Domingo';
        case 1: return 'Segunda';
        case 2: return 'Terça';
        case 3: return 'Quarta';
        case 4: return 'Quinta';
        case 5: return 'Sexta';
        case 6: return 'Sábado';
    }
}

var ASCENDING = 0;
var DESCENDING = 1;
function getGridSortParameters(gridName) {
    var grid = dijit.byId(gridName);
    var retorno = { sort: '', direction: null };

    if (hasValue(grid.sortInfo)) {
        // Verifica se e ascendente ou descentende:
        if (grid.sortInfo < 0)
            retorno.direction = DESCENDING;
        else
            retorno.direction = ASCENDING;

        //Pega o nome da coluna:
        retorno.sort = grid.structure[Math.abs(grid.sortInfo) - 1].field;
    }

    return retorno;
}

function getStrGridParameters(gridName) {
    var obj = getGridSortParameters(gridName);
    var retorno = '';

    if (obj.direction != null)
        retorno = "sort=" + obj.sort + "&direction=" + obj.direction + "&";
    else
        retorno = "sort=&direction=" + ASCENDING + "&";

    return retorno;
}

function diaSemana(data) {
    if (hasValue(data)) {
        var dia = data.getDay();
        var semana = new Array(6);
        semana[0] = 'Domingo';
        semana[1] = 'Segunda-Feira';
        semana[2] = 'Terça-Feira';
        semana[3] = 'Quarta-Feira';
        semana[4] = 'Quinta-Feira';
        semana[5] = 'Sexta-Feira';
        semana[6] = 'Sábado';
        return (semana[dia]);
    }
    return "";
}

// Método utilizado para arrendondar um determinado número com determinadas casas decimais. Ele é substituido pois o original ocorre erros, como para as seguintes entradas: parseFloat(10.55).toFixed(1) e parseFloat(10.555).toFixed(2)
Number.prototype.round = function(digits) {
    digits = Math.floor(digits);
    if (isNaN(digits) || digits === 0) {
        return Math.round(this);
    }
    if (digits < 0 || digits > 16) {
        throw 'RangeError: Number.round() digits argument must be between 0 and 16';
    }
    var multiplicator = Math.pow(10, digits);
    return Math.round(this * multiplicator) / multiplicator;
}
Number.prototype.toFixed = function (digits) {
    digits = Math.floor(digits);
    if (isNaN(digits) || digits === 0) {
        return Math.round(this).toString();
    }
    var parts = this.round(digits).toString().split('.');
    var fraction = parts.length === 1 ? '' : parts[1];
    if (digits > fraction.length) {
        fraction += new Array(digits - fraction.length + 1).join('0');
    }
    return parts[0] + '.' + fraction;
}

//Método para incluir a máscara para mostrar o valor no campo, já arrendodado:
//Este método somente aceita passagem como parâmetro um valor numérico.
function maskFixed(valor, nro) {
    if (!isNaN(valor) && valor != "")
        return parseFloat(valor).toFixed(nro).toString().replace('.', ',');
    else
        return 0;
}

//Método para excluir a máscara de campo, já arrendodado ou para arrendodar um campo numérico:
//Este método aceita como passagem de parâmetro um valor numérico ou string, mas retorna apenas valores numéricos.
function unmaskFixed(valor, nro) {
    if (hasValue(valor, true))
        return parseFloat(parseFloat(valor.toString().replace(',', '.')).toFixed(nro));
    else
        return 0;
}

/*-----------------------------------------------------------------------
Máscara para o campo data dd/mm/aaaa
Exemplo: <input maxlength="16" name="data" onKeyPress="mascaraDataHora(event, this)">
-----------------------------------------------------------------------*/
function mascaraDataHora(evento, objeto) {
    var keypress = (window.event) ? event.keyCode : evento.which;

    if (keypress == 17 || keypress == 67)
        return objeto.value;

    if (keypress == 17 || keypress == 86)
        return objeto.value;

    if (keypress != 46 && keypress != 8 && keypress != 39 && keypress != 37) {
        campo = dojo.byId(objeto.id);
        if (campo.value == '00/00/0000') {
            campo.value = ""
        }

        caracteres = '0123456789';
        separacao1 = '/';
        separacao2 = ' ';
        separacao3 = ':';
        conjunto1 = 2;
        conjunto2 = 5;
        conjunto3 = 10;
        conjunto4 = 13;
        conjunto5 = 16;

        if ((caracteres.search(String.fromCharCode(keypress)) != -1) && campo.value.length < (10)) {
            if (campo.value.length == conjunto1)
                campo.value = campo.value + separacao1;
            else if (campo.value.length == conjunto2)
                campo.value = campo.value + separacao1;
            else if (campo.value.length == conjunto3)
                campo.value = campo.value + separacao2;
            else if (campo.value.length == conjunto4)
                campo.value = campo.value + separacao3;
            else if (campo.value.length == conjunto5)
                campo.value = campo.value + separacao3;
        }
        else
            if (keypress != 9)
            event.returnValue = false;
    }
}

function dataAtualFormatada(dt) {
    if (!hasValue(dt))
        return "";

    var data = new Date(dt.replace('Z', '')),
        dia = data.getDate().toString().padStart(2, '0'),
        mes = (data.getMonth() + 1).toString().padStart(2, '0'), //+1 pois no getMonth Janeiro começa com zero.
        ano = data.getFullYear();
    return dia + "/" + mes + "/" + ano;
}

function statusAtivoFormatado(status) {
    if (status == true)
        return "Sim";
    if (status == false)
        return "Não";
}

function tipoMenuFormatado(tipo) {
    var data = [
            { name: "Secretaria", id: 1 },
            { name: "Coordenação", id: 2 },
            { name: "Financeiro", id: 3 },
            { name: "Gestão", id: 4 },
            { name: "ECommerce", id: 5 },
            { name: "Configurações", id: 6 },
            { name: "Cadastros Básicos", id: 7 },
            { name: "Informativos", id: 8 },
		    { name: "Portal do Professor", id: 9 },
		    { name: "Portal do Aluno", id: 10 }
        ];

    return data[tipo - 1].name;
}

function isEmail(email) {
    var exclude = /[^@\-\.\w]|^[_@\.\-]|[\._\-]{2}|[@\.]{2}|(@)[^@]*\1/;
    var check = /@[\w\-]+\./;
    var checkend = /\.[a-zA-Z]{2,5}$/;
    var obj = eval("document.forms[0][2]");
    if (hasValue(trim(email))) {
        if (((email.search(exclude) != -1) || (email.search(check)) == -1) || (email.search(checkend) == -1)) {
            caixaDialogo(MENSAGEM_AVISO, 'E-mail no formato inválido.', "");
            return false;
            obj.focus();
        }
        else { return true; }
    }
}

function tipoMesFormatado(tipo) {

    var meses = [{ name: "Janeiro", id: 1 },
                 { name: "Fevereiro", id: 2 },
                 { name: "Março", id: 3 },
                 { name: "Abril", id: 4 },
                 { name: "Maio", id: 5 },
                 { name: "Junho", id: 6 },
                 { name: "Julho", id: 7 },
                 { name: "Agosto", id: 8 },
                 { name: "Setembro", id: 9 },
                 { name: "Outubro", id: 10 },
                 { name: "Novembro", id: 11 },
                 { name: "Dezembro", id: 12 }
    ];
    return meses[tipo - 1].name;
};

function montarMeses(idComponente, Memory, on) {

    var dados = [{ name: "Janeiro", id: "1" },
                 { name: "Fevereiro", id: "2" },
                 { name: "Março", id: "3" },
                 { name: "Abril", id: "4" },
                 { name: "Maio", id: "5" },
                 { name: "Junho", id: "6" },
                 { name: "Julho", id: "7" },
                 { name: "Agosto", id: "8" },
                 { name: "Setembro", id: "9" },
                 { name: "Outubro", id: "10" },
                 { name: "Novembro", id: "11" },
                 { name: "Dezembro", id: "12" }
    ]
    var statusStore = new Memory({
        data: dados
    });
    dijit.byId(idComponente).store = statusStore;
    dijit.byId(idComponente).set("value", 1);
};

//#region Validação de hora
function trasformarHorasEmMinutos(hora) {
    var horaTotal = parseInt(hora.substr(0, 2));
    var minuto = parseInt(hora.substr(3, 2));
    var minutoTotal = (horaTotal * 60) + minuto;
    return minutoTotal;
}

function validarHoraView(timeIni, timeFim, apresentadorMsg) {
    var timeIni = dojo.byId(timeIni).value;
    var timeFim = dojo.byId(timeFim).value;

    var totalMinutosIni = trasformarHorasEmMinutos(timeIni);
    var totalMinutosFim = trasformarHorasEmMinutos(timeFim);

    if (totalMinutosIni >= totalMinutosFim) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroHoraInicialMaior);
        apresentaMensagem(apresentadorMsg, mensagensWeb);
        return false;
    }
    return true;
}

function validarDatasIniFim(dataIni, dataFim, apresenteadorMSG, date) {
    var dtaInicio = null;
    var dtaFinal = null;

    if (hasValue(dojo.byId(dataIni).value))
        dtaInicio = hasValue(dojo.byId(dataIni).value) ? dojo.date.locale.parse(dojo.byId(dataIni).value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
    if (hasValue(dojo.byId(dataFim).value))
        dtaFinal = hasValue(dojo.byId(dataFim).value) ? dojo.date.locale.parse(dojo.byId(dataFim).value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

    //Verifica se a data inicial é maior que a data final:
    if (dtaInicio != null & dtaFinal != null)
        if (date.compare(dtaInicio, dtaFinal) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinalBiblioteca);
            apresentaMensagem(apresenteadorMSG, mensagensWeb);
            return false;
        }

    return true;
}

function incluiNoCache(dc_menu) {
    var retorno = dc_menu;

    if (retorno.indexOf("abrePopUpURL") == -1) {
        if (retorno.indexOf("?") == -1)
            retorno = Endereco() + retorno + '?noCache=' + Math.floor(Math.random() * 16777215).toString(16);
        else
            retorno = Endereco() + retorno + '&noCache=' + Math.floor(Math.random() * 16777215).toString(16);
    }

    return retorno;
}
//#endregion

//#region sessão do usuário
var interval = new Number();
var minutos = new Number();
var segundos = new Number();
var solicitou_renovacao = false;
function formatatempo(segs, min) {

    while (segs >= 60) {
        if (segs >= 60) {
            segs = segs - 60;
            min = min + 1;
        }
    }

    //while (min >= 60) {
    //    if (min >= 60) {
    //        min = min - 60;
    //    }
    //}

    if (segs == 0 && min > 0) {
        if (min == 1) segs = 60;
        min--;
    }

    if (min < 10) { min = "0" + min }

    if (segs < 10) { segs = "0" + segs }

    fin = "Sua sessão expira em " + min + " min " + segs + " seg";

    if (!solicitou_renovacao && segs < 60 && min < 5)
        solicitaRenovacaoSessao();

    return fin;
}
function solicitaRenovacaoSessao() {
    solicitou_renovacao = true;
    caixaDialogo(DIALOGO_CONFIRMAR, "Sua sessão será expirada! Deseja renovar?", function () { renovaSessao() });
}

function renovaSessao() {
    dojo.xhr.post({
        url: Endereco() + "/auth/postrenovasessao",
        headers: { "Accept": "application/json", "Content-Type": "application/json" }
    }).then(function (data) {
        solicitou_renovacao = false;
    }, function (error) {
        window.location.href = Endereco() + "/auth/Login";
    });
}

function iniciarSessao() {
    if (segundos != 0)
        segundos--;
    else {
        if (minutos >= 1) {
            minutos--;
            segundos = 59;
        }
        else
            restart();
    }

    if (minutos == 0 && segundos == 0)
        restart();

    document.getElementById("counter").innerHTML = formatatempo(segundos, minutos);

    var counter_form = document.getElementById("counter_form");
    if (hasValue(counter_form))
        for (var i = 1; hasValue(counter_form) ; i++) {
            counter_form.innerHTML = formatatempo(segundos, minutos);
            counter_form = document.getElementById("counter_form_" + i);
        }
    else {
        counter_form = document.getElementById("counter_form_1");
        for (var i = 1; hasValue(counter_form) ; i++) {
            counter_form.innerHTML = formatatempo(segundos, minutos);
            counter_form = document.getElementById("counter_form_" + i);
        }
    }
}

function iniciaRelogio(seg) {
    seg = parseInt(seg);
    seg = seg - 1;// se for 30 começar 29:59
    minutos = Math.floor(seg / 60);
    segundos = seg % 60;
}

function iniciaContadorRegressivo() {
    interval = setInterval("iniciarSessao();", 1000);
}

function restart() {
    clearTimeout(interval);
    dojo.xhr.post({
        url: Endereco() + "/auth/postlogout",
        headers: { "Accept": "application/json", "Content-Type": "application/json" }
    }).then(function (data) {
        //caixaDialogo(DIALOGO_INFORMACAO, 'Tempo de sessão encerrado, efetue o login novamente.', executaRetorno);
        window.location.href = Endereco() + "/auth/Login";
    }, function (error) {
        window.location.href = Endereco() + "/auth/Login";
    });
}

function getSessionTime() {

    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            preventCache: false,
            url: Endereco() + "/api/auth/getTimeSession",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            data = jQuery.parseJSON(data);
            var results = data.retorno.split("Æ");
            minutos = parseInt(results[0].trim());
            segundos = parseInt(results[1].trim());
            iniciaContadorRegressivo();
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemUsuario', error);
        });
    });
}
//#endregion

function setMenssageMultiSelect(tipo, idComponente) {
    try {
        switch (tipo) {
            case PERIODO:
                configurarDescricaoCheckedMultiSelect("Escolher período(s).", "{num} período(s) selecionado(s)", dijit.byId(idComponente));
                break;
            case PRODUTO:
                configurarDescricaoCheckedMultiSelect("Escolher produto(s).", "{num} produto(s) selecionado(s)", dijit.byId(idComponente));
                break;
            case DIA:
                configurarDescricaoCheckedMultiSelect("Escolher dia da semana.", "{num} dia(s) da semana selecionado(s)", dijit.byId(idComponente));
                break;
            case SITUACAO:
                configurarDescricaoCheckedMultiSelect("Escolher Situação do Aluno.", "{num} situação(ões) selecionada(s)", dijit.byId(idComponente));
                break;
            case CURSO:
                configurarDescricaoCheckedMultiSelect("Escolher Curso.", "{num} situação(ões) selecionada(s)", dijit.byId(idComponente));
                break;
            case ESTAGIO:
                configurarDescricaoCheckedMultiSelect("Escolher Estágio.", "{num} estágio(s) selecionado(s)", dijit.byId(idComponente));
                break;
            case TPAVALIACAO:
                configurarDescricaoCheckedMultiSelect("Escolher Tipo Avaliação.", "{num} tipos(s) de avaliação selecionado(s)", dijit.byId(idComponente));
                break;
            case TITULOS:
                configurarDescricaoCheckedMultiSelect("Escolher Status do Titulo.", "{num} status selecionado(s)", dijit.byId(idComponente));
                break;
            case MES:
                configurarDescricaoCheckedMultiSelect("Escolher Mês.", "{num} mês(es) selecionado(s)", dijit.byId(idComponente));
                break;
            case MENUS:
                configurarDescricaoCheckedMultiSelect("Escolher Menu.", "{num} menu(s) selecionado(s)", dijit.byId(idComponente));
                break;
            case TPITEM:
                configurarDescricaoCheckedMultiSelect("Escolher Tipo Item.", "{num} tipo(s) de Item selecionado(s)", dijit.byId(idComponente));
                break;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setMenssageMultiSelect(tipo, idComponente, mostrarlabel, size) {
        switch (tipo) {
            case PERIODO:
                configurarDescricaoCheckedMultiSelect("Escolher período(s).", "{num} período(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case PRODUTO:
                configurarDescricaoCheckedMultiSelect("Escolher produto(s).", "{num} produto(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case DIA:
                configurarDescricaoCheckedMultiSelect("Escolher dia da semana.", "{num} dia(s) da semana selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case SITUACAO:
                configurarDescricaoCheckedMultiSelect("Escolher Situação do Aluno.", "{num} situação(ões) selecionada(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case CURSO:
                configurarDescricaoCheckedMultiSelect("Escolher Curso.", "{num} Curso(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case ESTAGIO:
                configurarDescricaoCheckedMultiSelect("Escolher Estágio.", "{num} estágio(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case TPAVALIACAO:
                configurarDescricaoCheckedMultiSelect("Escolher Tipo Avaliação.", "{num} tipos(s) de avaliação selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case TITULOS:
                configurarDescricaoCheckedMultiSelect("Escolher Status do Titulo.", "{num} status selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case MES:
                configurarDescricaoCheckedMultiSelect("Escolher Mês.", "{num} mês(es) selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case MENUS:
                configurarDescricaoCheckedMultiSelect("Escolher Menu.", "{num} menu(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
            case TPITEM:
                configurarDescricaoCheckedMultiSelect("Escolher Tipo Item.", "{num} tipo(s) de Item selecionado(s)", dijit.byId(idComponente), mostrarlabel, size);
                break;
        }
}

function setMenssageMultiSelectOpcao(tipo, idComponente, mostrarlabel, Opcao) {
    switch (tipo) {
    case PERIODO:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher período(s).", "{num} período(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel,  Opcao);
        break;
    case PRODUTO:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher produto(s).", "{num} produto(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
    case DIA:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher dia da semana.", "{num} dia(s) da semana selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
    case SITUACAO:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher Situação do Aluno.", "{num} situação(ões) selecionada(s)", dijit.byId(idComponente), mostrarlabel,  Opcao);
        break;
    case CURSO:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher Curso.", "{num} curso(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
    case ESTAGIO:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher Estágio.", "{num} estágio(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
    case TPAVALIACAO:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher Tipo Avaliação.", "{num} tipos(s) de avaliação selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
    case TITULOS:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher Status do Titulo.", "{num} status selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
    case MES:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher Mês.", "{num} mês(es) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
    case MENUS:
        configurarDescricaoCheckedMultiSelectOpcao("Escolher Menu.", "{num} menu(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
        case TPITEM:
            configurarDescricaoCheckedMultiSelect("Escolher Tipo Item.", "{num} tipo(s) de Item selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
        break;
    }
}

function configurarDescricaoCheckedMultiSelect(descricaoEscolher, descricaoNroRegistros, componente) {
    if (componente.value.length > 0) {
        componente._nlsResources.multiSelectLabelText = descricaoNroRegistros;
    }
    else
        componente._nlsResources.multiSelectLabelText = descricaoEscolher;
    componente._updateSelection();
}

function configurarDescricaoCheckedMultiSelect(descricaoEscolher, descricaoNroRegistros, componente, mostrarlabel, size) {
        if (componente.value.length > 0) {
            if (mostrarlabel == true) {
                componente._nlsResources.multiSelectLabelText = '';
                for (var i = 1; i < componente.options.length; i++) {
                    if (componente.options[i].selected == true) {
                        componente._nlsResources.multiSelectLabelText = componente._nlsResources.multiSelectLabelText +
                            componente.options[i].label +
                            ',';
                    }
                }

                if (componente._nlsResources.multiSelectLabelText.length > size) {
                    componente._nlsResources.multiSelectLabelText =
                        componente._nlsResources.multiSelectLabelText.substring(0, size) + "...";

                } else {
                    componente._nlsResources.multiSelectLabelText =
                        componente._nlsResources.multiSelectLabelText.slice(0, -1);
                }

            } else
                componente._nlsResources.multiSelectLabelText = descricaoNroRegistros;
        } else
            componente._nlsResources.multiSelectLabelText = descricaoEscolher;
        componente._updateSelection();
   
}


function configurarDescricaoCheckedMultiSelectOpcao(descricaoEscolher, descricaoNroRegistros, componente, mostrarlabel, Opcao) {
    if (componente.value.length > 0) {
        if (mostrarlabel == true) {
            componente._nlsResources.multiSelectLabelText = '';
            for (var i = Opcao; i < componente.options.length; i++) {
                if (componente.options[i].selected == true) {
                    if (i < componente.value.length && i < componente.options.length - 1)
                        componente._nlsResources.multiSelectLabelText = componente._nlsResources.multiSelectLabelText + componente.options[i].label + ',';
                    else
                        componente._nlsResources.multiSelectLabelText = componente._nlsResources.multiSelectLabelText + componente.options[i].label;
                }
            }
        }
        else
            componente._nlsResources.multiSelectLabelText = descricaoNroRegistros;
    }
    else
        componente._nlsResources.multiSelectLabelText = descricaoEscolher;
    componente._updateSelection();
}
//#endregion

//Função que expande todos os nós de uma arvore no Dojo LazyTreeGrid:
function expandAllDojoLazyTreeGrid(grid, levels) {
    for (var i = 0; i < levels; i++) {
        var v = grid.views.views[grid.views.views.length - 1];
        if (hasValue(v) && hasValue(v._expandos))
            for (var e in v._expandos)
                grid.expand(parseInt(e));
        else
            setTimeout(function () { expandAllDojoLazyTreeGrid(grid, levels); }, 100);
    }
}

function adicionarAtalhoPesquisa(listaCampos, botaoPesq, ready) {
    ready(function () {
        try {
            for (var i = 0; i < listaCampos.length; i++) {
                var campo = dijit.byId(listaCampos[i]);
                if (hasValue(campo, true))
                    dijit.byId(listaCampos[i]).on("keyup", function (e) {
                        if (e.keyCode == 13) {
                            dijit.byId(botaoPesq).focus();
                            dijit.byId(botaoPesq).onClick();
                        }
                    });
                else
                    throw new Exception("Chamada inválida para função 'adicionarAtalhoPesquisa' com parametro: " + listaCampos[i] + ' botão de pesquisa: ' + botaoPesq);
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function Exception(message) {
    this.message = message;
    this.name = "Exception";
}

function postGerarLog(e) {
    var stack = e.stack || e.stacktrace || "";
	
	if (hasValue(e, true) && hasValue(e.message)) {
		require(["dojox/json/ref"], function (ref) {
			dojo.xhr.post({
				url: Endereco() + "/util/postGerarLog",
				preventCache: true,
				headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
				postData: dojox.json.ref.toJson({ stack: "Message: " + e.message + " Stack: " + stack })
			}).then(function (data) {
			});
		});
		throw e;
	}
}

function efetuaLogout() {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.post({
            url: Endereco() + "/auth/postlogout",
            headers: { "Accept": "application/json", "Content-Type": "application/json" }
        }).then(function (data) {
            caixaDialogo(DIALOGO_INFORMACAO, 'Logout efetuado com sucesso.', executaRetorno);
        }, function (error) {
            caixaDialogo(DIALOGO_ERRO, error, null);
        });
    });
}

function executaRetorno() {
    //window.location = window.location;
    window.location = Endereco() + '/Auth/Login';
    //window.location.reload();
}

/**
* Função recursiva responsável pela criação do menu dinamicamente, do menu de segundo nível em diante:
*/
function mostraMenusFilhos(data, pSubMenu, MenuItem, DropDownMenu, MenuSeparator, id_empresa_propria) {
    if (hasValue(data))
        for (var j = 0; j < data.length; j++) {
            pSubMenuNovo = new DropDownMenu({});
            
            if (!eval(MasterGeral()) && data[j].no_menu == "Importação XML - Notas Fiscais") {
                continue;
            }
            // Relatório Matricula de outros materiais, somente master poderá executar.
            if (!eval(MasterGeral()) && data[j].no_menu == "Matricula outros Produtos") {
                continue;
            }
            if ((jQuery.parseJSON(MasterGeral()) && (!id_empresa_propria || (id_empresa_propria && (data[j].no_menu != "Faturamento" || data[j].no_menu != "Inventario")))) ||
                (!jQuery.parseJSON(MasterGeral()) && data[j].no_menu != "Grupo Master" && (!id_empresa_propria || (id_empresa_propria && (data[j].no_menu != "Faturamento" || data[j].no_menu != "Inventario"))))) {
                if (data[j].MenusFilhos.length > 0) {
                    
                    pSubMenu.addChild(new MenuItem({
                        dc_url_menu: incluiNoCache(data[j].dc_url_menu),
                        label: data[j].no_menu,
                        onClick: function (obj) { abreMenu(this.dc_url_menu); },
                        popup: pSubMenuNovo
                    }));
                    if (hasValue(data[j].id_separador))
                        pSubMenu.addChild(new MenuSeparator());
                }
                else {
                    pSubMenu.addChild(new MenuItem({
                        dc_url_menu: incluiNoCache(data[j].dc_url_menu),
                        label: data[j].no_menu,
                        onClick: function (obj) { abreMenu(this.dc_url_menu); }
                    }));
                    if (hasValue(data[j].id_separador))
                        pSubMenu.addChild(new MenuSeparator());
                }
            }
            mostraMenusFilhos(data[j].MenusFilhos, pSubMenuNovo, MenuItem, DropDownMenu, MenuSeparator, id_empresa_propria);
        }
}

//Função para remover o timezone da data:
function removeTimeZone(date) {
    if (hasValue(date) && date.getTimezoneOffset() != 180) { // Timezone do Brasil, horário de Brasília
        var dateSplit = date.toString().split("GMT");
        if(dateSplit.length > 1){
            return new Date(dateSplit[0] + "GMT-0300 (Hora Padrão Brasil Central)");
        }
    }
    return date;
}

function getNomeLabelRelatorio() {
    return nomeLabelRelatorioDinamico;
}

function parseFloatMascara(valor) {
    var aux = 0;
    for (var i = 0; i < valor.length; i++) {
        if (valor[i] == "." || valor[i] == ",")
            aux++;
    }

    if (aux == 2)
        return parseFloat((valor).replace(".", "").replace(",", "."));
    return parseFloat((valor).replace(",", "."));
}

function horaFormatada(hh) {
    var hh = hh.split(':');
    return hh[0] + ":" + hh[1];
}

require(["dojo/currency"]);

function formartValorDinheiro(valor) {
    formatado = dojo.currency.format(valor, { currency: "R$ " });
    console.log(formatado);
    return formatado
}

function verificaCookie(cookieObj) {
	try {

		return (hasValue(cookieObj) &&
			hasValue(cookieObj["cookieconsent_sgf_PERSONALIZATION"]) &&
			cookieObj["cookieconsent_sgf_PERSONALIZATION"] == "ALLOW" &&
			hasValue(cookieObj["cookieconsent_sgf_ESSENTIAL"]) &&
			cookieObj["cookieconsent_sgf_ESSENTIAL"] == "ALLOW" &&
			//hasValue(cookieObj["cookieconsent_status"]) &&
			//cookieObj["cookieconsent_status"] == "allow" &&
			hasValue(cookieObj["cookieconsent_sgf_UNCATEGORIZED"]) &&
			cookieObj["cookieconsent_sgf_UNCATEGORIZED"] == "ALLOW" &&
			hasValue(cookieObj["cookieconsent_sgf_ANALYTICS"]) &&
			cookieObj["cookieconsent_sgf_ANALYTICS"] == "ALLOW" &&
			hasValue(cookieObj["cookieconsent_sgf_MARKETING"]) &&
			cookieObj["cookieconsent_sgf_MARKETING"] == "ALLOW");

	} catch (e) {
		postGerarLog(e);
	}
}

function formataCookie(cookieStr) {
	try {
		return cookieStr.split(';').reduce(function (cookies, cookie) {
			cookies[cookie.split("=")[0].trim()] = unescape(cookie.split("=")[1]);
			return cookies;
		}, {});
	} catch (e) {
		postGerarLog(e);
	} 
}