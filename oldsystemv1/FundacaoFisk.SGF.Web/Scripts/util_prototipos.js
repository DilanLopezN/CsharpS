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

var dataHoje = new Date();
var dia = dataHoje.getDate();
var mes = dataHoje.getMonth();
var ano = dataHoje.getFullYear();
var hora = dataHoje.getHours();
var min = dataHoje.getMinutes();
var seg = dataHoje.getSeconds();

var PRODUTO = 0, PERIODO = 1, DIA = 2, SITUACAO = 3, ESTAGIO = 4, TPAVALIACAO = 5, TITULOS = 6, MES = 7, MENUS = 8;

//Monta Máscara para um campo de data
function maskDate(field) {
	$(field).mask("99/99/9999");
}

function maskHour(field) {
	$(field).mask("99:99:99");
};

function maskHourMin(field) {
    $(field).mask("99:99");
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
function loadPesqSexo(Memory, id) {
	require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
	 function (ready, Memory, filteringSelect) {
		 var stateStore = new Memory({
			 data: [
					 { name: "Todos", id: 0 },
					 { name: "Feminino", id: 1 },
					 { name: "Masculino", id: 2 },
					 { name: "Não Binário", id: 3 },
					 { name: "Prefiro não responder ou Neutro", id: 4 }
			 ]
		 });
		 id.store = stateStore;
		 id.set("value", 0);
	 });

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
	if ((!str && valor != null && valor != '' && valor != undefined && valor != 'null')
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
	if(hasValue(apresentadorMensagem)){
		apresentadorMensagem.style.display = "none";
		apresentadorMensagem.style.height = "";

		if (!mantem_mensagem)
			apresentadorMensagem.mensagensAnteriores = null;

		if (hasValue(error) && hasValue(error.response)) { //aconteceu um erro
			if (hasValue(data) && hasValue(data.MensagensWeb))
				mensagensWeb = data.MensagensWeb;
			else
				mensagensWeb = jQuery.parseJSON(data).MensagensWeb;
		}
		else if (hasValue(error) && hasValue(jQuery.parseJSON(error)) && hasValue(jQuery.parseJSON(error).MensagensWeb))// retornou uma msg qualquer
			mensagensWeb = jQuery.parseJSON(error).MensagensWeb;
		else if (hasValue(error) && hasValue(error) && hasValue(error.MensagensWeb))// retornou uma msg qualquer no mvc
			mensagensWeb = error.MensagensWeb;
		else if (hasValue(error) && hasValue(error[0]) && hasValue(error[0].tipoMensagem))// criou a mensagem no js da própria página web
			mensagensWeb = error;
		else if (hasValue(error) && hasValue(error.erro) && hasValue(error.erro) && hasValue(error.erro.MensagensWeb)) // ocorreu um erro no mvc.
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

// *** Função que alterar as  divs de alteração e Inclução fazendo com que as mesmas possam  desparecer ou aparecer conforme a ação do usuário. ***\\
function IncluirAlterar(valor, divAlterar, divIncluir, divExcluir, msg, divCancelar, divLimpar) {
	apresentaMensagem(msg, null);
	if (valor == 1) {
		document.getElementById(divIncluir).style.display = "";
		document.getElementById(divLimpar).style.display = "";
		document.getElementById(divCancelar).style.display = "none";
		document.getElementById(divAlterar).style.display = "none";
		document.getElementById(divExcluir).style.display = "none";
	}
	else {
		document.getElementById(divAlterar).style.display = "";
		document.getElementById(divCancelar).style.display = "";
		document.getElementById(divExcluir).style.display = "";
		document.getElementById(divLimpar).style.display = "none";
		document.getElementById(divIncluir).style.display = "none";
	}
}
//*** fim da função ***\\

//** Limpa os dados da pagina **\\
function getLimpar(form) {
	$(form).get(0).reset();
}

function caixaDialogo(ptipoDiag, pmsg, pFuncaoRetorno) {
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
					title: (ptipoDiag == DIALOGO_CONFIRMAR) ? 'Confirmar' : (ptipoDiag == DIALOGO_AVISO)?"Aviso": (ptipoDiag == DIALOGO_INFORMACAO)?"Informa\u00e7\u00e3o" : "Erro",
					hasSkipCheckBox: false,
					hasOkButton: (ptipoDiag==DIALOGO_CONFIRMAR),
					labelOkbutton: (ptipoDiag == DIALOGO_CONFIRMAR) ? 'Sim':'OK', 
					labelCancelbutton: (ptipoDiag == DIALOGO_CONFIRMAR) ? 'N\u00e3o':'OK',
					content: (ptipoDiag == DIALOGO_CONFIRMAR) ? '<p>'+ pmsg + '</p>' : '<p>'+ Imagem + pmsg + '</p>'
				});
				dialog.show().then(function (remember) {
					// user pressed ok button
					// remember is true, when user wants you to remember his decision (user ticked the check box)
					if (!remember)
						if (ptipoDiag == DIALOGO_CONFIRMAR && hasValue(pFuncaoRetorno, false)) 
							pFuncaoRetorno.call();
				}, function () {
					if (ptipoDiag == DIALOGO_INFORMACAO && hasValue(pFuncaoRetorno, false)) 
						pFuncaoRetorno.call();
					return false;
				})
			})
		})
}

// ** função que monta o comboBox de staus
function montarStatus(nomElement, simples) {
	require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
	 function (ready, Memory, filteringSelect) {
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
		 //ready(function () {
		 var status = new filteringSelect({
			 id: nomElement,
			 name: "status",
			 value: "1",
			 store: statusStore,
			 searchAttr: "name",
			 style: "width: 75px;"
		 }, nomElement);
		 //})
	 });
};

function montarStatusSingular(nomElement, simples) {
	require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
	 function (ready, Memory, filteringSelect) {
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
		 //ready(function () {
		 var status = new filteringSelect({
			 id: nomElement,
			 name: "status",
			 value: "1",
			 store: statusStore,
			 searchAttr: "name",
			 style: "width: 75px;"
		 }, nomElement);
		 //})
	 });
};

/*
 * Está Função server para criar o componente filteringSelect passando como parametro o elemento,os objetos de Dados, e o tamanho do compo em width. ex: width:100px
 */
function criarOuCarregarCompFiltering(nomElement, data, tamanho, value, ready, Memory, filteringSelect, no_codigo, no_nome,FiltroSexo) {
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
				name: eval("value." + no_nome)
			});
		});
	}

	var statusStore = new Memory({
		data: storeData
	});
	if (!hasValue(dijit.byId(nomElement),true))
		var status = new filteringSelect({
			id: nomElement,
			name: nomElement,
			store: statusStore,
			searchAttr: "name",
			value: value,
			style: tamanho
		}, nomElement);
	else
		dijit.byId(nomElement).store = statusStore;

	if (value != null && hasValue(value), true)
		dijit.byId(nomElement).set("value",value);
};

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
		if(hasValue(f) && hasValue(f.elements)) // Limpa os elementos de um formulário:
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
	require(["dojo/store/Memory", "dijit/form/FilteringSelect", "dijit/registry", "dojo/_base/array"],
		function (Memory, FilteringSelect, _registry, Array) {
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
		"dojo/dom", 
		"dojo/dom-attr", 
		"dojo/_base/xhr", 
		"dojox/json/ref",
		"dojo/ready",
		"dijit/registry",
		"dojo/data/ItemFileReadStore",
		"dojo/_base/array",
		"dijit/layout/TabContainer",
		"dijit/layout/ContentPane",
		"dojox/form/CheckedMultiSelect"
	], function (dom, domAttr, xhr, ref, ready, registry, ItemFileReadStore, BaseArray) {
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

function setMenssageMultiSelect(tipo, idComponente) {
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
	}
}

function setMenssageMultiSelect(tipo, idComponente, mostrarlabel) {
	switch (tipo) {
		case PERIODO:
			configurarDescricaoCheckedMultiSelect("Escolher período(s).", "{num} período(s) selecionado(s)", dijit.byId(idComponente),mostrarlabel);
			break;
		case PRODUTO:
			configurarDescricaoCheckedMultiSelect("Escolher produto(s).", "{num} produto(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel);
			break;
		case DIA:
			configurarDescricaoCheckedMultiSelect("Escolher dia da semana.", "{num} dia(s) da semana selecionado(s)", dijit.byId(idComponente), mostrarlabel);
			break;
		case SITUACAO:
			configurarDescricaoCheckedMultiSelect("Escolher Situação do Aluno.", "{num} situação(ões) selecionada(s)", dijit.byId(idComponente), mostrarlabel);
			break;
		case ESTAGIO:
			configurarDescricaoCheckedMultiSelect("Escolher Estágio.", "{num} estágio(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel);
			break;
		case TPAVALIACAO:
			configurarDescricaoCheckedMultiSelect("Escolher Tipo Avaliação.", "{num} tipos(s) de avaliação selecionado(s)", dijit.byId(idComponente), mostrarlabel);
			break;
		case TITULOS:
			configurarDescricaoCheckedMultiSelect("Escolher Status do Titulo.", "{num} status selecionado(s)", dijit.byId(idComponente), mostrarlabel);
			break;
		case MES:
			configurarDescricaoCheckedMultiSelect("Escolher Mês.", "{num} mês(es) selecionado(s)", dijit.byId(idComponente), mostrarlabel);
			break;
		case MENUS:
			configurarDescricaoCheckedMultiSelect("Escolher Menu.", "{num} menu(s) selecionado(s)", dijit.byId(idComponente),mostrarlabel);
			break;
	}
}

function setMenssageMultiSelect(tipo, idComponente, mostrarlabel, Opcao) {
	switch (tipo) {
		case PERIODO:
			configurarDescricaoCheckedMultiSelect("Escolher período(s).", "{num} período(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
			break;
		case PRODUTO:
			configurarDescricaoCheckedMultiSelect("Escolher produto(s).", "{num} produto(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
			break;
		case DIA:
			configurarDescricaoCheckedMultiSelect("Escolher dia da semana.", "{num} dia(s) da semana selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
			break;
		case SITUACAO:
			configurarDescricaoCheckedMultiSelect("Escolher Situação do Aluno.", "{num} situação(ões) selecionada(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
			break;
		case ESTAGIO:
			configurarDescricaoCheckedMultiSelect("Escolher Estágio.", "{num} estágio(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
			break;
		case TPAVALIACAO:
			configurarDescricaoCheckedMultiSelect("Escolher Tipo Avaliação.", "{num} tipos(s) de avaliação selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
			break;
		case TITULOS:
			configurarDescricaoCheckedMultiSelect("Escolher Status do Titulo.", "{num} status selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
			break;
		case MES:
			configurarDescricaoCheckedMultiSelect("Escolher Mês.", "{num} mês(es) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
			break;
		case MENUS:
			configurarDescricaoCheckedMultiSelect("Escolher Menu.", "{num} menu(s) selecionado(s)", dijit.byId(idComponente), mostrarlabel, Opcao);
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

function configurarDescricaoCheckedMultiSelect(descricaoEscolher, descricaoNroRegistros, componente, mostrarlabel) {
	if (componente.value.length > 0) {
		if (mostrarlabel == true) {
			componente._nlsResources.multiSelectLabelText = '';
			for (var i = 1; i < componente.options.length; i++) {
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

function configurarDescricaoCheckedMultiSelect(descricaoEscolher, descricaoNroRegistros, componente, mostrarlabel, Opcao) {
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

//*********************************************************************************************************
//	Função para abrir o popup:
//*********************************************************************************************************
function abrePopUp(caminho_pagina, tam_horizontal, tam_vertical) {
	janela = window.open(caminho_pagina, 'popup', 'width=' + tam_horizontal + ', height=' + tam_vertical + ' scrollbars=yes, menubar=no, resizable=yes, status=no, titlebar=no, toolbar=no,top=100,left=50');
	janela.window.moveTo(125, 100);
	janela.focus();
}

//*********************************************************************************************************
//	Função para abrir o popup:
//*********************************************************************************************************
function abrePopUpSemResizable(caminho_pagina, tam_horizontal, tam_vertical) {
	janela = window.open(caminho_pagina, 'popup', 'width=' + tam_horizontal + ', height=' + tam_vertical + ' scrollbars=yes, menubar=no, resizable=no, status=no, titlebar=no, toolbar=no,top=100,left=50');
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

//=============================================================================

//============================================================================
// Funções para manipular em uma lista js de forma perfomática usando quicksort - log(n):
Array.prototype.swap=function(a, b) {
	var tmp=this[a];
	this[a]=this[b];
	this[b]=tmp;
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
	var piv=array[pivot];
	array.swap(pivot, end-1);
	var store=begin;
	var ix;

	for (ix = begin; ix < end - 1; ++ix) {
		var arrayAux = parseFloat(array[ix]);
		var pivAux = parseFloat(piv);

		if (arrayAux+'' != 'NaN')
			array[ix] = arrayAux;
		if (pivAux + '' != 'NaN')
			piv = pivAux;

		if (array[ix] <= piv) {
			array.swap(store, ix);
			++store;
		}
	}
	array.swap(end-1, store);

	return store;
}

function partitionObj(array, campo, begin, end, pivot) {
	var piv = eval('array[pivot].'+campo);
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

function qsort(array, begin, end)
{
	if(end-1>begin) {
		var pivot = begin + Math.floor(Math.random()*(end-begin));
		//var pivot = begin + Math.floor((end - begin)/2);

		pivot=partition(array, begin, end, pivot);
		qsort(array, begin, pivot);
		qsort(array, pivot+1, end);
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

function quickSort(array)
{
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
	return removeObjSortAux(array, campo, value, 0, array.length-1);
}

function removeObjSortAux(array, campo, value, inicio, fim) {
	var pivot = inicio + Math.floor((fim - inicio) / 2);
	var valor = null;

	if (hasValue(array) && array.length > 0)
		valor = eval('array[pivot].' + campo);
		
	if(hasValue(valor)){
		if(valor == value)
			return array.splice(pivot, 1);
		else if(valor > value && pivot - 1 >= 0)
			removeObjSortAux(array, campo, value, inicio, pivot - 1);
		else if (pivot + 1 <= array.length && inicio <= fim)
			removeObjSortAux(array, campo, value, pivot + 1, fim);
	}
}

// Função para incluir e retornar uma lista separada por ponto e virgula de forma ordenada:
function insertStrSort(str, value, duplicate)
{
	var array=str.split(';');

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
	return binarySearchAux(array, value, 0, array.length-1);
}

// Função que retorna a posição do objeto no array ordenado se existir ou nulo quando não existir.
function binaryObjSearch(array, campo, value) {
	return binaryObjSearchAux(array, campo, value, 0, array.length - 1);
}

function binarySearchAux(array, value, inicio, fim) {
	var retorno = false;
	var pivot = inicio + Math.floor((fim - inicio) / 2);

	if (hasValue(array) && array.length > 0 && inicio >= 0 && fim < array.length){
		if ((array.length == 1 && array[0] == value) || array[pivot] == value)
			retorno = true;
		else
			if (array[pivot] < value && pivot + 1 <= fim)
				return binarySearchAux(array, value, pivot + 1, fim);
			else if (pivot -1 >= 0 && inicio <= fim)
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
		if ((array.length == 1 && eval('array[0].'+campo) == value) || eval('array[pivot].'+campo) == value)
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
			removeObjSort(grid.itensSelecionados, field, eval('grid.getItem(rowIndex).'+field));
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
			else if(hasValue(dojo.byId(idTodos)))
				dojo.byId(idTodos).parentNode.removeChild(dojo.byId(idTodos));
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
	if(hasValue(marcarTodos) && hasValue(evt.srcElement) && hasValue(evt.srcElement.value) && evt.srcElement.value == 'Infinity')
		marcarTodos.destroy();
}


function setGridPagination(grid, item, campo) {
	// Busca pelo nome da coluna que a grid quando está ordanada:
	//if (hasValue(grid.sortInfo))
	//    campo = grid.layout.cells[grid.sortInfo-1].field;

	// Busca a posição do item no grid de forma perfomática:
	var posicao = binaryObjSearch(grid.store.objectStore.data, campo, eval('item.'+campo));

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
		  "dojo/store/Memory",
		  "dojo/dom"
	], function (ObjectStore, Memory, dom) {
		var todos = dojo.byId(todosItensName+"_label");
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
			windowUtils.scrollIntoView(widget.containerNode || widget.domNode);
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
	grid.store.save();
	var dados = grid.store.objectStore.data;

	if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {
		//Percorre a lista da grade para deleção (O(n)):
		for (var i = dados.length - 1; i >= 0; i--)
			// Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
			if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].'+nomeId)) != null)
				dados.splice(i, 1); // Remove o item do array

		grid.itensSelecionados = new Array();
		var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
		grid.setStore(dataStore);
		grid.update();
	}
	else
		caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
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

	for (var i = 0; i < array.length; i++)
		retorno.push(clone(array[i]));

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

	if(hasValue(grid.sortInfo)){
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
			event.returnValue = false;
	}
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

	if(retorno.indexOf("?") == -1)
		retorno = retorno + '?noCache=' + Math.floor(Math.random() * 16777215).toString(16);
	else
		retorno = retorno + '&noCache=' + Math.floor(Math.random() * 16777215).toString(16);

	return retorno;
}
//#endregion