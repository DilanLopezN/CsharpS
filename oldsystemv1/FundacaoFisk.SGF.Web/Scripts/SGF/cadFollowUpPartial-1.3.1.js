var INTERNO = 1, PROSPECTALUNO = 2, ADMINISTRACAOGERAL = 3, ALUNO = 4;
var TODOS = 0, PESQUISAESCOLAFKFOLLOWUP = 10, PAIS_BRASIL = 1;
var msgAulaDemonstrativa = "Foi agendada uma aula demonstrativa para o dia xx/xx/xxxx das h_inicial as h_final. O link da aula será enviado posteriormente.";
var msgAulaDemonstrativaAux = null;

var EnumAcaoFollowUp = {
    AULADEMONSTRATIVA: 10
};

function formatCheckBoxEscolaFollowUpFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridEscolasFollowUpFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEscolaFollowUpFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoEscolaFollowUpFK', -1, 'selecionaTodosEscolaFollowUpFK', 'selecionaTodosEscolaFollowUpFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoEscolaFollowUpFK', " + rowIndex + ", '" + id + "', 'selecionaTodosEscolaFollowUpFK', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaFollowUpFK(funcao) {
    require([
			"dijit/Editor",
            "dijit/_editor/plugins/FontChoice",
            "dijit/_editor/plugins/TextColor",
            "dijit/_editor/plugins/LinkDialog",
            "dijit/_editor/plugins/ViewSource",
            "dojox/editor/plugins/ShowBlockNodes",
            "dojox/editor/plugins/Preview",
            "dojox/editor/plugins/Blockquote",
            "dijit/_editor/plugins/NewPage",
            "dojox/editor/plugins/FindReplace",
            "dojox/editor/plugins/ToolbarLineBreak",
            "dojox/editor/plugins/CollapsibleToolbar",
            "dijit/form/Button",
            "dijit/form/DropDownButton",
            "dijit/DropDownMenu",
            "dijit/MenuItem",
            "dojo/ready",
            "dojo/dom",
            "dojox/grid/EnhancedGrid",
            "dojox/grid/enhanced/plugins/Pagination",
            "dojo/data/ObjectStore",
            "dojo/store/Memory",
            "dijit/registry"
    ],
            function (Editor, FontChoice, TextColor, LinkDialog, ViewSource, ShowBlockNodes, Preview, Blockquote, NewPage, FindReplace, ToolbarLineBreak, CollapsibleToolbar,
                Button, DropDownButton, DropDownMenu, MenuItem, ready, dom, EnhancedGrid, Pagination, ObjectStore, Memory) {
                ready(function () {
                    try {
                        inserirIdTabsCadastroFollowUpPartial();
                        alterarVisibilidadeEscolas(false);
                        loadCadTipoFollowUpFK(null, false);
                        if (hasValue(dijit.byId("mensagemFollowUppartial"))) {
                            dijit.byId("mensagemFollowUppartial").destroyRecursive();
                            $('<div>').attr('id', 'mensagemFollowUppartial').appendTo('#paneMensagemFollowUpFK');
                        }
                        var editor = new Editor({
                            height: '200px',
                            plugins: ['collapsibletoolbar', 'undo', 'redo', '|', 'cut', 'copy', 'paste', '|', 'bold', 'italic', 'underline', 'strikethrough', '|', 'insertOrderedList', 'insertUnorderedList', 'indent', 'outdent',
                                                      '|', 'justifyLeft', 'justifyRight', 'justifyCenter', 'justifyFull', '|', 'foreColor', 'hiliteColor', '|', 'createLink', 'insertImage', '|',
                                                      'newpage', /*'save',*/'showBlockNodes', 'preview', 'findreplace',
                                                      { name: 'viewSource', stripScripts: true, stripComments: true }, 'selectAll', '|', 'fontName', 'fontSize', { name: 'formatBlock', plainText: true }]
                        }, "mensagemFollowUppartial");
                        editor.startup();

                        var valid = true;
                        var maxLimit = 2048;
                        //dojo.connect(editor, "onKeyUp", this, function (event) {
                        //    if (editor.get("value").length > maxLimit) {
                        //        var mensagensWeb = new Array();
                        //        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Limite de caracteres de 2048 para o texto da mensagem foi ultrapassada.");
                        //        apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
                        //    }
                        //});

                        dijit.byId("mensagemFollowUppartial").set("required", true);
                        //Redimensiona os botões do editor de texto:
                        var botoes_editor = dojo.byId('mensagemFollowUppartial').children[0].children[0];
                        for (var i = 0; i < botoes_editor.children[0].children[0].children[2].children[0].children[0].children.length; i++)
                            if (hasValue(botoes_editor.children[0].children) && hasValue(botoes_editor.children[0].children[0])
                                    && hasValue(botoes_editor.children[0].children[0].children) && hasValue(botoes_editor.children[0].children[0].children[2])
                                    && hasValue(botoes_editor.children[0].children[0].children[2].children) && hasValue(botoes_editor.children[0].children[0].children[2].children[0])
                                    && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children) && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0])
                                    && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children)
                                    && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i])
                                    && botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].localName != "div") {
                                botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].style.minWidth = '32px';
                                if (hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children)
                                        && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children[0]))
                                    botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children[0].style.minWidth = '32px';
                            }
                        if (dijit.registry.byId("btnIncluirFollowUpPartial") == null) { new Button({ label: "Enviar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { } }, "btnIncluirFollowUpPartial"); }
                        new Button({ label: "Enviar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { } }, "alterarFollowUpPartial");
                        new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { } }, "cancelarFollowUpPartial");
                        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { } }, "btnFecharFollowUpPartial");
                        new Button({
                            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { }
                        }, "deleteFollowUpPartial");
                        new Button({
                            label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparCadFollowUpPartial(); }
                        }, "btnLimparFollowUpFK");
                        var btnAlunoProspectFK = dijit.byId("btnAlunoProspectFK");
                        if (hasValue(btnAlunoProspectFK.handler))
                            btnAlunoProspectFK.handler.remove();
                        btnAlunoProspectFK.handler = btnAlunoProspectFK.on("click", function (e) {
                            montarFKProspect();
                        });

                        decreaseBtn(document.getElementById("btnAlunoProspectFK"), '18px');
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
                                        montarGridPesquisaUsuarioFK(function () {
                                            abrirUsuarioFKFollowUp(CADUSERDESTINOFOLLOWUPFK);
                                        });
                                    else
                                        abrirUsuarioFKFollowUp(CADUSERDESTINOFOLLOWUPFK);
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "FKUsuarioDestinoCad");

                        new Button({
	                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
	                        onClick: function () {
		                        try {
			                        if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
				                        montarGridPesquisaUsuarioFK(function () {
                                            abrirUsuarioFKFollowUp(CADPROSPECTALUNOUSERDESTINOFOLLOWUPFK);
				                        });
			                        else
                                        abrirUsuarioFKFollowUp(CADPROSPECTALUNOUSERDESTINOFOLLOWUPFK);
		                        } catch (e) {
			                        postGerarLog(e);
		                        }
	                        }
                        }, "FKProspectAlunoUsuarioDestinoCad");

                        new Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true,
                            onClick: function () {
                                try {
                                    dijit.byId("cadNomeUsuarioDestinoFollowUpFK").reset();
                                    dojo.byId("cdUsuarioDestinoFollowUpFK").value = 0;
                                    dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", true);
                                    dijit.byId("ckAdmFollowUpFK").set("disabled", false);
                                    dijit.byId("ckAdmFollowUpFK").reset();
                                    if (eval(MasterGeral()))
                                        alterarVisibilidadeEscolas(false);
                                    else
                                        alterarVisibilidadeEscolas(true);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "limparCadUsuarioDestinoFollowUpFK");

                        new Button({
	                        label: "Limpar", iconClass: '', type: "reset", disabled: true,
	                        onClick: function () {
		                        try {
                                    dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").reset();
                                    dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value = 0;
                                    dijit.byId("limparCadProspectAlunoUsuarioDestinoFollowUpFK").set("disabled", true);
			                        dijit.byId("ckAdmFollowUpFK").set("disabled", false);
			                        dijit.byId("ckAdmFollowUpFK").reset();
			                        if (eval(MasterGeral()))
				                        alterarVisibilidadeEscolas(false);
			                        else
				                        alterarVisibilidadeEscolas(true);
		                        }
		                        catch (e) {
			                        postGerarLog(e);
		                        }
	                        }
                        }, "limparCadProspectAlunoUsuarioDestinoFollowUpFK");

                        decreaseBtn(document.getElementById("limparCadProspectAlunoUsuarioDestinoFollowUpFK"), '40px');
                        decreaseBtn(document.getElementById("FKProspectAlunoUsuarioDestinoCad"), '18px');

                        decreaseBtn(document.getElementById("limparCadUsuarioDestinoFollowUpFK"), '40px');
                        decreaseBtn(document.getElementById("FKUsuarioDestinoCad"), '18px');
                        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { } }, "btnFecharFollowUpFK");

                        // Adiciona link de ações:
                        var menuAcoesEscolaFollowUpFK = new DropDownMenu({ style: "height: 25px" });
                        var acaoRemoverFollowUpFK = new MenuItem({
                            label: "Excluir",
                            onClick: function () {
                                try {
                                    if (hasValue(dojo.byId("cd_follow_up_partial").value) && dojo.byId("cd_follow_up_partial").value > 0 && dijit.byId("ckLidoFollowUpFK").get("checked")) {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é possível alterar follow-up já lido.");
                                        apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
                                    } else
                                        deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_pessoa', dijit.byId("gridEscolasFollowUpFK"));
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        });
                        menuAcoesEscolaFollowUpFK.addChild(acaoRemoverFollowUpFK);
                        var buttonEscolaFollowUpFK = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadas",
                            dropDown: menuAcoesEscolaFollowUpFK,
                            id: "acoesRelacionadasEscolasFollowUpFK"
                        });
                        dom.byId("linkAcoesEscolaFollowUpFK").appendChild(buttonEscolaFollowUpFK.domNode);
                        new Button({
                            label: "Incluir",
                            iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                            onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                        montargridPesquisaPessoa(function () {
                                            dojo.query("#_nomePessoaFK").on("keyup", function (e) {
                                                if (e.keyCode == 13)
                                                    pesquisarEscolasFKFollowUpFK();
                                            });
                                            dijit.byId("pesqPessoa").on("click", function (e) {
                                                apresentaMensagem("apresentadorMensagemProPessoa", null);
                                                pesquisarEscolasFKFollowUpFK();
                                            });
                                            abrirPessoaFKFollowUpPartial(false);
                                            montarPesquisaLocalidade();
                                        });
                                    else
                                        abrirPessoaFKFollowUpPartial(false);
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "incluirEscolaFKFollowUpFK");
                        dijit.byId('ckLidoFollowUpFK').on('change', function (e) {
                            marcarFollowUpComoLido(e);
                            //if (dijit.byId('ckResolvidoFollowUpFK').checked)
                        });
                        criarOuCarregarCompFiltering("cadTipoAtendimento", [{ name: "Pessoal", id: "1" }, { name: "Telefônico ", id: "2" }, { name: "E-mail ", id: "3" }], "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
                        if (hasValue(funcao))
                            funcao.call();

                        dijit.byId("cadAcaoFollowUpFK").on("change", function (e) {
                            if (e == EnumAcaoFollowUp.AULADEMONSTRATIVA) {
                                dojo.byId('paneTurmaFollowUpFK').style.display = '';

                                dijit.byId("dtaProxContatoFollowUpFK").set("required", true);
                                dojo.byId("cbTurma").required = true;

                                populaProduto(dojo.xhr);
                                populaCursos(dojo.xhr);
                                loadPopulaCompProf(dojo.xhr, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect);
                                if (hasValue(msgAulaDemonstrativaAux))
                                    dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativaAux);
                                else
                                    dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativa);
                            } else {
                                dijit.byId("dtaProxContatoFollowUpFK").set("required", false);
                                dojo.byId("cbTurma").required = false;

                                dojo.byId('paneTurmaFollowUpFK').style.display = 'none';
                                msgAulaDemonstrativaAux = null;
                                if (hasValue(dijit.byId("mensagemFollowUppartial").value) &&
                                    dijit.byId("mensagemFollowUppartial").value
                                    .indexOf("Foi agendada uma aula demonstrativa para o dia") !==
                                    -1) {
                                    dijit.byId("mensagemFollowUppartial").set("value", "");
                                }
                                
                                limparTurma();
                            }
                        });

                        dijit.byId("dtaProxContatoFollowUpFK").on("change", function (e) {
                            //var data = dojo.date.locale.parse(e, { formatLength: 'short', selector: 'date', locale: 'pt-br' })

                            if (hasValue(e)) {
                                if (hasValue(dojo.byId("cd_turma_pesquisa").value) && dojo.byId("cd_turma_pesquisa").value > 0) {
                                    verificaDiaSemanaTurmaFollowUp(e);
                                } else {
                                    apresentaMensagem('apresentadorMensagemCadFollowUpPartial', null);
                                    if (hasValue(msgAulaDemonstrativaAux) && hasValue(dijit.byId("cadAcaoFollowUpFK").value) && dijit.byId("cadAcaoFollowUpFK").value === EnumAcaoFollowUp.AULADEMONSTRATIVA) {

                                        if (dijit.byId("mensagemFollowUppartial").value.indexOf("xx/xx/xxxx") == -1) {//se ja fez replace, acha a posicao da data 
                                            dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativaAux.replace(msgAulaDemonstrativaAux.substring(47, (47 + 10)),
                                                dojo.date.locale.format(e, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })));
                                        } else {
                                            dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativaAux.replace("xx/xx/xxxx",
                                                dojo.date.locale.format(e, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })));
                                        }
                                        
                                    } else if (!hasValue(msgAulaDemonstrativaAux) && hasValue(dijit.byId("cadAcaoFollowUpFK").value) && dijit.byId("cadAcaoFollowUpFK").value === EnumAcaoFollowUp.AULADEMONSTRATIVA){
                                        dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativa.replace("xx/xx/xxxx",
                                        dojo.date.locale.format(e, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })));
                                    }
                                    
                                    
                                }
                            }
                        });

                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                
                                if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                    montarGridPesquisaTurmaFK(function () {
                                        abrirTurmaFK();
                                        dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                                montarGridPesquisaAluno(false, function () {
                                                    debugger
                                                    abrirAlunoFKTurmaFK(true);
                                                });
                                            }
                                            else
                                                abrirAlunoFKTurmaFK(true);
                                        });
                                    });
                                else
                                    abrirTurmaFK();
                                apresentaMensagem('apresentadorMensagemCadFollowUpPartial', null);
                            }
                        }, "pesProTurmaFK");

                        new Button({
                            label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                                limparTurma();
                            }
                        }, "limparProTurmaFK");

                        if (hasValue(document.getElementById("limparProTurmaFK"))) {
                            document.getElementById("limparProTurmaFK").parentNode.style.minWidth = '40px';
                            document.getElementById("limparProTurmaFK").parentNode.style.width = '40px';
                        }
                        btnPesquisar(dojo.byId("pesProTurmaFK"));
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

            });
}

function montarPesquisaLocalidade(funcao) {
    showP('trLocalidade', true);
    //Popula os estados e cidades:
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/localidade/GetEstadoByPais?cd_pais=" + PAIS_BRASIL,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            if (hasValue(data.retorno))
                criarOuCarregarCompFiltering("cb_estado_fk", data.retorno, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');
            if (hasValue(funcao))
                funcao.call();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemLogradouro', error);
    });

    dijit.byId("cb_estado_fk").on("change", function (cdEstado) {
        try {
            if (!hasValue(cdEstado) || cdEstado < TODOS)
                dijit.byId("pesCidadeEscolaFK").set("disabled", true);
            else
                dijit.byId("pesCidadeEscolaFK").set("disabled", false);

            //Limpa a cidade:
            limpaCidadeEscolaFK();
        }
        catch (e) {
            postGerarLog(e);
        }
    });

    dijit.byId("pesCidadeEscolaFK").on("click", function (e) {
        try {
            if (!hasValue(dijit.byId("gridPesquisaCidade")))
                montargridPesquisaCidade(function () {
                    abrirPesquisaCidadeFKLogradouroFK();
                    dojo.query("#nomeCidade").on("keyup", function (e) { if (e.keyCode == 13) pesquisaCidadeFK(); });
                    dijit.byId("pesquisar").on("click", function (e) {
                        pesquisaCidadeFK();
                    });
                });
            else
                abrirPesquisaCidadeFKLogradouroFK();
        }
        catch (e) {
            postGerarLog(e);
        }
    });

    dijit.byId("limparCidadeEscolaFK").on("click", function (e) {
        try {
            dojo.byId('cb_cidade_fk').value = '';
            dojo.byId('cd_cidade_escolaFK').value = '';
            dijit.byId("limparCidadeEscolaFK").set('disabled', true);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
    decreaseBtn(document.getElementById("limparCidadeEscolaFK"), '40px');

    dijit.byId("pesCidadeEscolaFK").set("disabled", true);
    decreaseBtn(document.getElementById("pesCidadeEscolaFK"), '18px');
}

function abrirPesquisaCidadeFKLogradouroFK() {
    try {
        limparFiltrosCidaddeFK();
        var compPaisCidadeFK = dijit.byId("paisFk");
        var cdEstado = dijit.byId("cb_estado_fk").value;
        compPaisCidadeFK._onChangeActive = false;
        compPaisCidadeFK.set("value", PAIS_BRASIL);
        compPaisCidadeFK.set("disabled", true);
        compPaisCidadeFK._onChangeActive = false;
        if (hasValue(cdEstado)) {
            carregarEstadoPorPais(PAIS_BRASIL, function () {
                dijit.byId("estadoFk").set("value", cdEstado);
                dijit.byId("estadoFk").set("disabled", true);
                pesquisaCidadeFK(true);
                dijit.byId("cadCidadeEscolaFK").show();
            });
        }
        if (hasValue(dojo.byId("id_origem_cidade")))
            dojo.byId("id_origem_cidade").value = TIPO_PESQUISA_FOLLOW_ESCOLA_FK;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarCidadeEscolaFK() {
    try {
        var gridPesquisaCidade = dijit.byId("gridPesquisaCidade");
        if (!hasValue(gridPesquisaCidade.itensSelecionados) || gridPesquisaCidade.itensSelecionados.length <= 0 || gridPesquisaCidade.itensSelecionados.length > 1) {
            if (gridPesquisaCidade.itensSelecionados != null && gridPesquisaCidade.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaCidade.itensSelecionados != null && gridPesquisaCidade.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            return false;
        }

        var value = gridPesquisaCidade.itensSelecionados;
        if (value.length > 0) {
            $("#cd_cidade_escolaFK").val(value[0].cd_cidade);
            $("#cb_cidade_fk").val(value[0].no_cidade);
        }
        dijit.byId('limparCidadeEscolaFK').set("disabled", false);
        dijit.byId("cadCidadeEscolaFK").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function limpaCidadeEscolaFK() {
    try {
        dojo.byId('cd_cidade_escolaFK').value = 0;
        dojo.byId("cb_cidade_fk").value = "";
        dijit.byId('limparCidadeEscolaFK').set("disabled", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarFKProspect(p_funcao) {
    try {
        if (!hasValue(dijit.byId("fecharProspectFK"))) {
            montarGridPesquisaProspect(true, function () {
                abrirFKProspectFollowUpPartial();
                //dojo.query("#nomeProspectFK").on("keyup", function (e) {
                //    if (e.keyCode == 13) pesquisarProspectFK();
                //});
                dijit.byId("fecharProspectFK").on("click", function (e) {
                    dijit.byId("cadProspectFollowUpFK").hide();
                });

                if (hasValue(p_funcao))
                    p_funcao.call();
            }, true);
        }
        else {
            abrirFKProspectFollowUpPartial();
            if (hasValue(p_funcao))
                p_funcao.call();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function inserirIdTabsCadastroFollowUpPartial() {
    try {
        if (hasValue(dojo.byId("tabContainerFollowUpFK_tablist_tabEscolasFollowUpFK")))
            dojo.byId("tabContainerFollowUpFK_tablist_tabEscolasFollowUpFK").parentElement.id = "paiTabEscolasFollowUpFK";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTabFollowUpFK(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainerFollowUpFK_tablist_tabEscolasFollowUpFK' && !hasValue(dijit.byId("gridEscolasFollowUpFK"))) {
            if (hasValue(dojo.byId("cd_follow_up_partial").value) && dojo.byId("cd_follow_up_partial").value > 0) {
                loadEscolasFollowUpPartial(dojo.byId("cd_follow_up_partial").value);
            } else { criarGradeEscolaFollowUpPartial(null); }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadCadTipoFollowUpFK(value, edit) {
    try {
        var tipoFollowUpFK = dijit.byId("cadTipoFollowUpFK");
        var storeTipo = null;
        if (edit)
            storeTipo = new dojo.store.Memory({
                data: [
                  { name: "Interno", id: "1" },
                  { name: "Prospect/Aluno", id: "2" },
                  { name: "Administração Geral", id: "3" }
                ]
            });
        else {
            if (eval(MasterGeral()))
                storeTipo = new dojo.store.Memory({
                    data: [
                      { name: "Interno", id: "1" },
                      { name: "Prospect/Aluno", id: "2" },
                      { name: "Administração Geral", id: "3" }
                    ]
                });
            else
                storeTipo = new dojo.store.Memory({
                    data: [
                      { name: "Interno", id: "1" },
                      { name: "Prospect/Aluno", id: "2" }
                    ]
                });
        }
        tipoFollowUpFK.store = storeTipo;
        if (value != null && value > 0)
            tipoFollowUpFK.set("value", value);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Criação das funções das FK's.
var ORIGCADALUNO = 1, ORIGPESQFOLLOWUP = 2, ORIGCADFOLLOWUP = 3;
function retornarProspectFKFollowUpPartial() { //selecionaProspectFK
    try {
        var valido = true;
        var gridPesquisaProspect = dijit.byId("gridPesquisaProspect");

        if (!hasValue(gridPesquisaProspect.itensSelecionados) || gridPesquisaProspect.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (gridPesquisaProspect.itensSelecionados.length > 1) {
                caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
                valido = false;
            }
            else {
                if (hasValue(gridPesquisaProspect.itensSelecionados[0].cd_prospect) || hasValue(gridPesquisaProspect.itensSelecionados[0].cd_aluno)) {
                    dijit.byId('descAlunoProspectFollowUpFK').set("value", gridPesquisaProspect.itensSelecionados[0].no_pessoa);
                    if (hasValue(gridPesquisaProspect.itensSelecionados[0].cd_prospect))
                        dojo.byId('cdProspectFollowUpPartial').value = gridPesquisaProspect.itensSelecionados[0].cd_prospect;
                    if (hasValue(gridPesquisaProspect.itensSelecionados[0].cd_aluno))
                        dojo.byId('cdAlunoFollowUpPartial').value = gridPesquisaProspect.itensSelecionados[0].cd_aluno;
                    dijit.byId("cadEmailProspectAluno").set("value", gridPesquisaProspect.itensSelecionados[0].email);
                    dijit.byId("cadTelefoneProspectAluno").set("value", gridPesquisaProspect.itensSelecionados[0].telefone);
                    //dijit.byId("limparProspect").set("disabled", false);
                }
            }
        if (!valido)
            return false;
        dijit.byId("cadProspectFollowUpFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirFKProspectFollowUpPartial() {
    try {
        limparPesquisaProspectFK();
        dojo.byId("cadProspectFollowUpFK_title").innerHTML = "Pesquisa de Prospect/Aluno";
        dojo.byId("idOrigemProspectFK").value = ORIGCADFOLLOWUP;
        dojo.byId("trTipoPesquisaFKProspect").style.display = "";
        dojo.byId("panelProspectFK").style.height = '390px';
        dojo.byId("paiPanelProspectFK").style.height = '395px';
        dijit.byId("tipoPesquisaFKProspect").set("value", PESQUISARPROSPECT);
        pesquisarProspectFKOrigemFollowUp(false);
        dijit.byId("cadProspectFollowUpFK").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirUsuarioFKFollowUp(cdOrigChamadaFKUsuario) {
    try {
        dojo.byId("idOrigemUsuarioFK").value = cdOrigChamadaFKUsuario;
        limparPesquisaUsuarioFK();
        pesquisarUsuarioFKFollowUpPesq(cdOrigChamadaFKUsuario);
        dijit.byId("proUsuario").show();
        apresentaMensagem('apresentadorMensagemProUsuario', null);
    } catch (e) {
        postGerarLog(e);
    }

}

function pesquisarUsuarioFKFollowUpPesq(tipoPesq) {
    require([
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/domReady!",
    "dojo/parser"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            myStore = Cache(
            JsonRest({
                target: Endereco() + "/Escola/getUsuarioSearchFKFollowUp?descricao=" + encodeURIComponent(document.getElementById("pesquisatextFK").value) + "&nome=" + encodeURIComponent(document.getElementById("nomPessoaFK").value) + "&inicio=" + document.getElementById("inicioUsuarioFK").checked + "&tipoPesq=" + tipoPesq,
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridUsuario = dijit.byId("gridPesquisaUsuarioFK");
            gridUsuario.setStore(dataStore);
            gridUsuario.itensSelecionados = [];

            gridUsuario.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function retornarUsuarioFKFollowUpPartial() {
    try {
        var valido = true;
        var gridUsuarioSelec = dijit.byId("gridPesquisaUsuarioFK");
        var gridUsuarios = dijit.byId("gridUsuarioGrupo");
        if (!hasValue(gridUsuarioSelec.itensSelecionados))
            gridUsuarioSelec.itensSelecionados = [];
        if (!hasValue(gridUsuarioSelec.itensSelecionados) || gridUsuarioSelec.itensSelecionados.length <= 0 || gridUsuarioSelec.itensSelecionados.length > 1) {
            if (gridUsuarioSelec.itensSelecionados != null && gridUsuarioSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridUsuarioSelec.itensSelecionados != null && gridUsuarioSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        } else {
            var tipoOrigem = dojo.byId("idOrigemUsuarioFK").value;
            if (hasValue(tipoOrigem))
                switch (parseInt(tipoOrigem)) {
                    case CADUSERDESTINOFOLLOWUPFK:
                            dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("value", gridUsuarioSelec.itensSelecionados[0].no_login);
                            dojo.byId("cdUsuarioDestinoFollowUpFK").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario;
                            dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", false);
                            dijit.byId("ckAdmFollowUpFK").set("disabled", true);
                            dijit.byId("ckAdmFollowUpFK").reset();
                            destroyCreateGridEscolasFollowUpPartial();
                            alterarVisibilidadeEscolas(false);
                        break;
					case CADPROSPECTALUNOUSERDESTINOFOLLOWUPFK:
                            dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").set("value", gridUsuarioSelec.itensSelecionados[0].no_login);
                            dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario;
                            dijit.byId("limparCadProspectAlunoUsuarioDestinoFollowUpFK").set("disabled", false);
	                        dijit.byId("ckAdmFollowUpFK").set("disabled", true);
	                        dijit.byId("ckAdmFollowUpFK").reset();
	                        destroyCreateGridEscolasFollowUpPartial();
	                        alterarVisibilidadeEscolas(false);
                        break;
                    default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                        return false;
                        break;
                }
        }
        if (!valido)
            return false;
        dijit.byId("proUsuario").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarProspectFKOrigemFollowUp(vinculoFollowUp) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var cdTipoPesqFollowUp = hasValue(dijit.byId("tipoPesquisaFKProspect").value) ? dijit.byId("tipoPesquisaFKProspect").value : PESQUISARPROSPECT;
            var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/secretaria/getProspectAlunoFKSearch?nome=" + dojo.byId('nomeProspectFK').value + "&inicio=" + dijit.byId("inicioProspectFK").checked + "&email="
                                   + dojo.byId('emailPesqProspFK').value + "&telefone=" + dojo.byId('telefoneProspectFK').value + "&tipoPesquisa=" + cdTipoPesqFollowUp + "&vinculoFollowUp=" + vinculoFollowUp,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_prospect"
            }), Memory({ idProperty: "cd_prospect" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridProspect = dijit.byId("gridPesquisaProspect");
            gridProspect.itensSelecionados = [];
            gridProspect.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirPessoaFKFollowUpPartial(isPesquisa) {
    dojo.ready(function () {
        try {
            dojo.byId("setValuePesquisaPessoaFKFollowUp").value = PESQUISAESCOLAFKFOLLOWUP;
            dijit.byId("proPessoa").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            limparPesquisaEscolaFKFollowUpPartial();
            pesquisarEscolasFKFollowUpFK();
            dijit.byId("proPessoa").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaEscolaFKFollowUpPartial() {
    try {
        dojo.byId("_nomePessoaFK").value = "";
        dojo.byId("_apelido").value = "";
        dojo.byId("CnpjCpf").value = "";
        if (hasValue(dijit.byId("gridPesquisaPessoa"))) {
            dijit.byId("gridPesquisaPessoa").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoa").itensSelecionados))
                dijit.byId("gridPesquisaPessoa").itensSelecionados = [];
        }

        dojo.byId("cb_cidade_fk").value = '';
        dojo.byId("cd_cidade_escolaFK").value = '';

        dijit.byId("cb_estado_fk").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarEscolasFKFollowUpFK() {
    if (hasValue(dojo.byId("setValuePesquisaPessoaFKFollowUp")) && dojo.byId("setValuePesquisaPessoaFKFollowUp").value && dojo.byId("setValuePesquisaPessoaFKFollowUp").value == PESQUISAESCOLAFKFOLLOWUP)
        require([
         "dojo/store/JsonRest",
         "dojo/data/ObjectStore",
         "dojo/store/Cache",
         "dojo/store/Memory",
         "dojo/ready",
         "dojo/domReady!",
         "dojo/parser"],
     function (JsonRest, ObjectStore, Cache, Memory, ready) {
         ready(function () {
             try {
                 var cb_estado_fk = hasValue(dijit.byId('cb_estado_fk')) ? dijit.byId('cb_estado_fk').value : '';
                 var cd_cidade_escolaFK = hasValue(dojo.byId('cd_cidade_escolaFK')) ? dojo.byId('cd_cidade_escolaFK').value : '';
                 var myStore = Cache(
                      JsonRest({
                          target: Endereco() + "/escola/getEscolaSearchFKFollowUp?nome=" + dojo.byId("_nomePessoaFK").value +
                                        "&fantasia=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                        "&cnpj=" + dojo.byId("CnpjCpf").value + "&cd_estado=" + cb_estado_fk + "&cd_cidade=" + cd_cidade_escolaFK,
                          handleAs: "json",
                          headers: { "Accept": "application/json", "Authorization": Token() }
                      }), Memory({}));

                 dataStore = new ObjectStore({ objectStore: myStore });
                 var grid = dijit.byId("gridPesquisaPessoa");
                 grid.setStore(dataStore);
                 grid.layout.setColumnVisibility(5, false)
             } catch (e) {
                 postGerarLog(e);
             }
         });
     });
}

function setarTabCadFollowUpPartial() {
    try {
        var tabs = dijit.byId("tabContainerFollowUpFK");
        var pane = dijit.byId("tabPrincipalFollowUpFK");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridEscolasFollowUpPartial() {
    try {
        if (hasValue(dijit.byId("gridEscolasFollowUpFK"))) {
            dijit.byId("gridEscolasFollowUpFK").destroyRecursive();
            $('<div>').attr('id', 'gridEscolasFollowUpFK').attr('style', 'height:410px;').appendTo('#panelEscolasFKFollowUpFK');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadEscolasFollowUpPartial(cd_follow_up_partial) {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/escola/getEscolasFollowUp?cd_follow_up=" + parseInt(cd_follow_up_partial),
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataEscolas) {
            try {
                //if (hasValue(dataHorario.retorno) && dataHorario.retorno.length > 0) {
                //    $.each(dataHorario.retorno, function (idx, val) {
                //        if (val.cd_registro != cdAluno) {
                //            if (val.id_situacao == 1)
                //                val.calendar = "Calendar2";
                //            if (val.id_situacao == 9)
                //                val.calendar = "Calendar5";
                //        }
                //        //dojo.date.locale.parse("0"+val.DiaSemana +"/07/2012 " + val.fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
                //        val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                //        val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });

                //        //Pega o menor horário:
                //        if (menorData == null || val.dt_hora_ini < menorHorario) {
                //            menorData = val.startTime;
                //            menorHorario = val.dt_hora_ini;
                //        }
                //    });
                //    dijit.byId("gridAluno").horarioAluno = dataHorario.retorno.slice();

                //    //Aciona uma thread para verificar se acabou de criar toda a grade horária e posicionar na primeira linha:
                //    setTimeout(function () { posicionaPrimeiraLinhaHorarioAluno(dataHorario.retorno.length, menorData); }, 100);
                //}
                criarGradeEscolaFollowUpPartial(dataEscolas.retorno);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarGradeEscolaFollowUpPartial(data) {
    try {
        if (hasValue(dijit.byId("gridEscolasFollowUpFK")))
            destroyCreateGridEscolasFollowUpPartial();
        var gridEscolasFollowUpFK = new dojox.grid.EnhancedGrid({
            store: new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }),
            structure: [
                {
                    name: "<input id='selecionaTodosEscolaFollowUpFK' style='display:none'/>", field: "selecionadoEscolaFollowUpFK", width: "25px", styles: "text-align: center;",
                    formatter: formatCheckBoxEscolaFollowUpFK
                },
                { name: "Escola", field: "dc_reduzido_pessoa", width: "98%" }
            ],
            noDataMessage: msgNotRegEnc,
            plugins: {
                pagination: {
                    pageSizes: ["17", "34", "68", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "17",
                    gotoButton: true,
                    maxPageStep: 4,
                    position: "button",
                    plugins: { nestedSorting: true }
                }
            }
        }, "gridEscolasFollowUpFK");
        gridEscolasFollowUpFK.canSort = function (col) { return Math.abs(col) != 1; };
        gridEscolasFollowUpFK.startup();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarVisibilidadeEscolas(bool) {
    try {
        if (bool)
            dojo.style("paiTabEscolasFollowUpFK", "display", "");
        else
            dojo.style("paiTabEscolasFollowUpFK", "display", "none");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoa() {
    try {
        if (hasValue(dojo.byId("setValuePesquisaPessoaFKFollowUp").value) && parseInt(dojo.byId("setValuePesquisaPessoaFKFollowUp").value) == PESQUISAESCOLAFKFOLLOWUP) {
            var valido = true;
            var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
            var gridEscolasFollowUpFK = dijit.byId("gridEscolasFollowUpFK");
            if (!hasValue(gridPessoaSelec.itensSelecionados))
                gridPessoaSelec.itensSelecionados = [];
            if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0) {
                if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                valido = false;
            }
            else {
                var value = gridPessoaSelec.itensSelecionados;
                var storeGridEscolas = (hasValue(gridEscolasFollowUpFK) && hasValue(gridEscolasFollowUpFK.store.objectStore.data)) ? gridEscolasFollowUpFK.store.objectStore.data : [];
                quickSortObj(storeGridEscolas, 'cd_escola');
                $.each(gridPessoaSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(storeGridEscolas, "cd_escola", {
                        cd_escola: value.cd_pessoa,
                        dc_reduzido_pessoa: value.dc_reduzido_pessoa
                    });
                });
                gridEscolasFollowUpFK.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: storeGridEscolas }) }));
            }
            if (!valido)
                return false;
            dijit.byId("proPessoa").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function marcarFollowUpComoLido(bool) {
    try {
        //if (dijit.byId('ckResolvidoFollowUpFK').checked)
        //id_follow_resolvido: dijit.byId("ckResolvidoFollowUpFK").get("checked"),
        var cd_follow_up_partial = dojo.byId("cd_follow_up_partial").value;
        var id_tipo_follow_up = dijit.byId("cadTipoFollowUpFK").get("value");
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/secretaria/postMarcarFollowUpComoLido",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({
                cd_follow_up: cd_follow_up_partial,
                id_follow_lido: bool,
                id_tipo_follow: id_tipo_follow_up
            })
        }).then(function (data) {
            data = jQuery.parseJSON(data);
            if (hasValue(data.erro) || !hasValue(data.retorno))
                apresentaMensagem('apresentadorMensagemCadFollowUpPartial', data);
            showCarregando();
        }, function (error) {
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodos C.R.U.D FollowUp

function limparCadFollowUpPartial() {
    try {
        //clearForm("formFollowUpFKPrincipal");
        dijit.byId('cadFollowUp').set('title', 'Cadastro de Follow-Up');

        apresentaMensagem('apresentadorMensagemCadFollowUpPartial', "");
        //apresentaMensagem("apresentadorMensagemCadFollowUpPartial", null);
        setarTabCadFollowUpPartial();
        destroyCreateGridEscolasFollowUpPartial();
        dojo.byId("cdUsuarioOrigFollowUpFK").value = null;
        dojo.byId("cd_follow_up_partial").value = null;
        dojo.byId("cdProspectFollowUpPartial").value = null;
        dojo.byId("cdAlunoFollowUpPartial").value = null;
        dojo.byId("cdUsuarioDestinoFollowUpFK").value = null;
        dijit.byId("cadNomeUsuarioOrigFollowUpFK").reset();
        dijit.byId("cadNomeUsuarioDestinoFollowUpFK").reset();
        dijit.byId("descAlunoProspectFollowUpFK").reset();

        dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value = null;
        dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").reset();

        if (hasValue(dijit.byId("FKUsuarioDestinoCad")))
            dijit.byId("FKUsuarioDestinoCad").set("disabled", false);

        if (hasValue(dijit.byId("FKProspectAlunoUsuarioDestinoCad")))
            dijit.byId("FKProspectAlunoUsuarioDestinoCad").set("disabled", false);

        dijit.byId("btnAlunoProspectFK").set("disabled", false);
        dijit.byId("dtaCadastroFollowUpFK").reset();
        dijit.byId("ckAdmFollowUpFK").reset();
        dijit.byId("ckResolvidoFollowUpFK")._onChangeActive = false;
        dijit.byId("ckResolvidoFollowUpFK").reset();
        dijit.byId("ckResolvidoFollowUpFK")._onChangeActive = true;
        dijit.byId("ckLidoFollowUpFK")._onChangeActive = false;
        dijit.byId("ckLidoFollowUpFK").reset();
        dijit.byId("ckLidoFollowUpFK")._onChangeActive = true;
        dijit.byId("dtaProxContatoFollowUpFK").reset();
        dijit.byId("cadAcaoFollowUpFK").reset();
        dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").reset();
        dijit.byId("ckAdm").reset();
        dijit.byId("panePesqGeralFollowUpFK").set("open", true);
        dijit.byId("mensagemFollowUppartial").set("value", "");
        dijit.byId("cadTipoAtendimento").reset();
        dojo.byId('cd_follow_up_pai').value = "";
        dojo.byId('cd_follow_up_origem').value = "";
        dijit.byId("cadTelefoneProspectAluno").reset();
        dijit.byId("cadEmailProspectAluno").reset();

        // Turma no follow-up
        dojo.byId("cd_turma_pesquisa").value = 0;
        dojo.byId("cbTurma").value = "";
        if (hasValue(dijit.byId('limparProTurmaFK')))
            dijit.byId('limparProTurmaFK').set("disabled", true);

        var compHistMensagem = dojo.byId("historicoMensagens");
        if (hasValue(compHistMensagem) && hasValue(compHistMensagem.children) && compHistMensagem.children.length > 0) {
            var lengthTagsHistorico = compHistMensagem.children.length;
            for (var i = 0; i < lengthTagsHistorico; i++)
                if (hasValue(dijit.byId("panelMensagem" + i))) {
                    dijit.byId("panelMensagem" + i).destroy();
                    $("#" + "panelMensagem" + i).remove();
                }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarFollowUpCadastro() {
    try {
        var retorno = true;

        if (hasValue(dojo.byId("cd_turma_pesquisa").value <= 0 && !hasValue(dojo.byId("cbTurma").value)) &&
            dijit.byId("cadAcaoFollowUpFK").value === EnumAcaoFollowUp.AULADEMONSTRATIVA) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Para o tipo 'Aula Demosntrativa, o campo 'Turma' é obrigatório." );
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            retorno = false;
        }

        if (!hasValue(dijit.byId("dtaProxContatoFollowUpFK").value) &&
            dijit.byId("cadAcaoFollowUpFK").value === EnumAcaoFollowUp.AULADEMONSTRATIVA) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Para o tipo 'Aula Demosntrativa, o campo 'Data Próximo Contato' é obrigatório.");
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            retorno = false;
        }

        
        
    /*--Solicitação -> 307427--*/
        if (!Master()) {
	        if (hasValue(dojo.byId("cdUsuarioOrigFollowUpFK").value) && dojo.byId("cdUsuarioOrigFollowUpFK").value != CODUSUARIOLOGADO) {
		        var mensagensWeb = new Array();
		        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroAlterarFollowUpDeOutroUsuario);
		        apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
		        retorno = false;
			}
        
        }
        if (parseInt(dijit.byId("cadTipoFollowUpFK").value) == INTERNO || parseInt(dijit.byId("cadTipoFollowUpFK").value) == ADMINISTRACAOGERAL)
            var escolas = null;
        var gridEscolasFollowUpFK = dijit.byId("gridEscolasFollowUpFK");
        if (hasValue(gridEscolasFollowUpFK) && gridEscolasFollowUpFK.store.objectStore.data != null)
            escolas = gridEscolasFollowUpFK.store.objectStore.data;
        if (!hasValue(dijit.byId('mensagemFollowUppartial').value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroMensagemFollowUpObrig);
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            retorno = false;
        }
        if (!dijit.byId("formFollowUpFKPrincipal").validate()) {
            setarTabCadFollowUpPartial();
            retorno = false;
        }
        switch (parseInt(dijit.byId("cadTipoFollowUpFK").value)) {
            case INTERNO:
                if ((eval(MasterGeral()) && !hasValue(dijit.byId("cadNomeUsuarioDestinoFollowUpFK").value)) || (!Master())) {
                    retorno = false;
                    //var mensagensWeb = new Array();
                    //mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Favor informar o usuário de destino.");
                    //apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
                }
                break;
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function novoFollowUpPartial(pFuncao) {
    dojo.ready(function () {
        try {
            IncluirAlterar(1, 'divAlterarFollowUpPartial', 'divIncluirFollowUpPartial', 'divExcluirFollowUpPartial', 'apresentadorMensagemCadFollowUpPartial', 'divCancelarFollowUpPartial', 'divClearFollowUpPartial');
            limparCadFollowUpPartial();
            findComponentesNovoFollowUpPartial(function () {
                dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("value", eval(LoginUsuario()));
                dijit.byId("cadFollowUp").show();
                if (hasValue(pFuncao))
                    pFuncao.call();
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function findComponentesNovoFollowUpPartial(pFuncao) {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/componentesNovoFollowUp",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = hasValue(data) && hasValue(data.retorno) ? data.retorno : null;
                if (data != null && data.acaoeFollowUp != null)
                    criarOuCarregarCompFiltering("cadAcaoFollowUpFK", data.acaoeFollowUp, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_acao_follow_up', 'dc_acao_follow_up');
                if (hasValue(pFuncao))
                    pFuncao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarHistoricoMensagensAnteriores(data) {
    try {
        if (hasValue(data) && data.length > 0)
            require(["dijit/TitlePane", "dojo/dom", "dojo/domReady!"], function (TitlePane, dom) {
                $.each(data, function (idx, value) {
                    var idComp = "panelMensagem" + idx;
                    $('#historicoMensagens').append("<div id=" + idComp + "></div>");
                    var tp = new TitlePane({ title: value.no_usuario_origem + " " + value.dta_follow_up, content: value.dc_assunto, open: false, id: idComp });
                    dom.byId(idComp).appendChild(tp.domNode);
                    tp.startup();
                });
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaProduto(xhr) {
    xhr.get({
        url: Endereco() + "/api/Coordenacao/getProdutoTurmaFollowUp?hasDependente=" + 7 + "&cd_produto=null",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Authorization": Token() }
    }).then(function (dataProdAtivo) {
        try {
            //loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'pesquisaProduto');
            loadSelect(jQuery.parseJSON(dataProdAtivo).retorno, "cd_produto_atual_follow", 'cd_produto', 'no_produto', 0);
        } catch (e) {
            postGerarLog(e);
        }
    }, function (error) {
        apresentaMensagem('apresentadorMensagemTipoAvaliacao', error);
    });
}

function populaCursos(xhr) {
    xhr.get({
        preventCache: true,
        url: Endereco() + "/api/curso/getCursosFollowUp",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            if (hasValue(data.retorno) && data.retorno.length > 0)
                loadSelect(data.retorno, "cd_curso_atual_follow", 'cd_curso', 'no_curso', 0);
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemMat', error);
    })
}

function abrirTurmaFK() {
    try {
        limparFiltrosTurmaFK();
        dojo.byId("idOrigemCadastro").value = TURMAFOLLOWUP;
        dijit.byId("proTurmaFK").titleBar.innerText = 'Pesquisar Turma On-Line';
        dijit.byId("proTurmaFK").show();
        dijit.byId("tipoTurmaFK").store.remove(0);

        if (hasValue(dijit.byId('pesProfessor'))) {
            dijit.byId('pesProfessorFK').set('value', dijit.byId("pesProfessor").value);
            dijit.byId("pesProfessorFK").set("disabled", true);
        } else {
            dijit.byId('pesProfessorFK').set('value', 0);
            dijit.byId("pesProfessorFK").set("disabled", false);
        }

        if (hasValue(dijit.byId('cd_curso_atual_follow'))) {
            dijit.byId('pesCursoFK').set('value', dijit.byId("cd_curso_atual_follow").value);
            dijit.byId("pesCursoFK").set("disabled", true);
        } else {
            dijit.byId('pesCursoFK').set('value', 0);
            dijit.byId("pesCursoFK").set("disabled", false);
        }

        if (hasValue(dijit.byId('cd_produto_atual_follow'))) {
            dijit.byId('pesProdutoFK').set('value', dijit.byId("cd_produto_atual_follow").value);
            dijit.byId("pesProdutoFK").set("disabled", true);
        } else {
            dijit.byId('pesProdutoFK').set('value', 0);
            dijit.byId("pesProdutoFK").set("disabled", false);
        }

        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
        pesquisarTurmaFK(0, TURMAFOLLOWUP, getDiaSemanaTurma());
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFK(fieldID, fieldNome) {
    try {
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("cd_turma_pesquisa").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dojo.byId("cbTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dijit.byId('limparProTurmaFK').set("disabled", false);

            if (hasValue(gridPesquisaTurmaFK.itensSelecionados[0].horarios)) {
                if (gridPesquisaTurmaFK.itensSelecionados[0].horarios.length > 1) {
	                //Verifica se a sequencia de horarios é contínua
                    var sequenciaContinua = gridPesquisaTurmaFK.itensSelecionados[0].horarios.every(function(value, index) {
		                return !hasValue(gridPesquisaTurmaFK.itensSelecionados[0].horarios[index + 1]) ||
		                (hasValue(gridPesquisaTurmaFK.itensSelecionados[0].horarios[index + 1]) &&
			                value.dt_hora_fim ==
			                gridPesquisaTurmaFK.itensSelecionados[0].horarios[index + 1].dt_hora_ini);
	                });

                    var dt_hora_ini_aux = "";
                    var dt_hora_fim_aux = "";

                    if (sequenciaContinua) {
	                    //Caso a turma tenha mais de 1 intervalo de horarios pega a minima hora inicial
	                     dt_hora_ini_aux = gridPesquisaTurmaFK.itensSelecionados[0].horarios.map((value) => {
		                    return value.dt_hora_ini;
	                    }).sort()[0];
	                    //Caso a turma tenha mais de 1 intervalo de horarios pega a maxima hora final
	                     dt_hora_fim_aux = gridPesquisaTurmaFK.itensSelecionados[0].horarios.map((value) => {
		                    return value.dt_hora_fim;
	                    }).sort().reverse()[0];
                    } else {
	                    //Se não for continua pega o primeiro intervalo
                        dt_hora_ini_aux = gridPesquisaTurmaFK.itensSelecionados[0].horarios[0].dt_hora_ini;
                        dt_hora_fim_aux = gridPesquisaTurmaFK.itensSelecionados[0].horarios[0].dt_hora_fim;
                    }

                    if (dijit.byId("mensagemFollowUppartial").value.indexOf("xx/xx/xxxx") == -1 ||
                        dijit.byId("mensagemFollowUppartial").value.indexOf("h_inicial") == -1 ||
                        dijit.byId("mensagemFollowUppartial").value.indexOf("h_final") == -1) {

                        dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativa);

                        dijit.byId("mensagemFollowUppartial").set("value",
                            dijit.byId("mensagemFollowUppartial").value.replace("h_inicial",
	                            dt_hora_ini_aux));

                        dijit.byId("mensagemFollowUppartial").set("value",
                            dijit.byId("mensagemFollowUppartial").value.replace("h_final",
	                            dt_hora_fim_aux));

                        if (hasValue(dijit.byId("dtaProxContatoFollowUpFK").value)) {
                            dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("xx/xx/xxxx",
                                dojo.date.locale.format(dijit.byId("dtaProxContatoFollowUpFK").value, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })));
                        }

                        msgAulaDemonstrativaAux = dijit.byId("mensagemFollowUppartial").value;
                    } else {
                        dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_inicial",
	                        dt_hora_ini_aux));

                        dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_final",
	                        dt_hora_fim_aux));

                        msgAulaDemonstrativaAux = dijit.byId("mensagemFollowUppartial").value;

                    }


                }

                if (gridPesquisaTurmaFK.itensSelecionados[0].horarios.length <= 1 ) {
                    

                    if (dijit.byId("mensagemFollowUppartial").value.indexOf("xx/xx/xxxx") == -1 ||
                        dijit.byId("mensagemFollowUppartial").value.indexOf("h_inicial") == -1 ||
                        dijit.byId("mensagemFollowUppartial").value.indexOf("h_final") == -1) {

                        dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativa);

                        dijit.byId("mensagemFollowUppartial").set("value",
                            dijit.byId("mensagemFollowUppartial").value.replace("h_inicial",
                                gridPesquisaTurmaFK.itensSelecionados[0].horarios[0].dt_hora_ini));

                        dijit.byId("mensagemFollowUppartial").set("value",
                            dijit.byId("mensagemFollowUppartial").value.replace("h_final",
                                gridPesquisaTurmaFK.itensSelecionados[0].horarios[0].dt_hora_fim));

                        if (hasValue(dijit.byId("dtaProxContatoFollowUpFK").value)) {
                            dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("xx/xx/xxxx",
                                dojo.date.locale.format(dijit.byId("dtaProxContatoFollowUpFK").value, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })));
                        }

                        msgAulaDemonstrativaAux = dijit.byId("mensagemFollowUppartial").value;
                    } else {
                        dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_inicial",
                            gridPesquisaTurmaFK.itensSelecionados[0].horarios[0].dt_hora_ini));

                        dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_final",
                            gridPesquisaTurmaFK.itensSelecionados[0].horarios[0].dt_hora_fim));

                        msgAulaDemonstrativaAux = dijit.byId("mensagemFollowUppartial").value;
                        
                    }
                    
                    
                }
                
            }
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadPopulaCompProf(xhr, ready, Memory, FilteringSelect) {
    try {
        var compProf = dijit.byId("pesProfessor");

        if (compProf != null && compProf.store.data.length == 0)
            xhr.get({
                url: Endereco() + "/api/professor/getAllProfessor",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    (hasValue(jQuery.parseJSON(data).retorno)) ? data = jQuery.parseJSON(data).retorno : jQuery.parseJSON(data);
                    criarOuCarregarCompFiltering("pesProfessor", data, "", null, ready, Memory, FilteringSelect, 'cd_pessoa', 'no_pessoa');
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemDiarioPartial', error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparTurma() {
    dojo.byId("cd_turma_pesquisa").value = 0;
    dojo.byId("cbTurma").value = "";
    dijit.byId('limparProTurmaFK').set("disabled", true);
    apresentaMensagem('apresentadorMensagem', null);
}

function getDiaSemanaTurma() {
    if (hasValue(dojo.byId('dtaProxContatoFollowUpFK').value)) {
        var day = new Date(dojo.date.locale.parse(dojo.byId("dtaProxContatoFollowUpFK").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' })).getDay();
        return (day + 1);
    }
    return 0;
}

function verificaDiaSemanaTurmaFollowUp(dataProximoContato) {
    showCarregando();
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/turma/verificaDiaSemanaTurmaFollowUp?cdTurma=" + dojo.byId("cd_turma_pesquisa").value +
                "&idDiaSemanaTurma=" + getDiaSemanaTurma(),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_pessoa"
        }).then(function (horario) {
            try {
                if (hasValue(horario) && horario.length <= 1) {
                    apresentaMensagem('apresentadorMensagemCadFollowUpPartial', "");
                    if (hasValue(msgAulaDemonstrativaAux)) {
                        if (dijit.byId("mensagemFollowUppartial").value.indexOf("xx/xx/xxxx") == -1) {//se ja fez replace, acha a posicao da data 
                            dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativaAux.replace(msgAulaDemonstrativaAux.substring(47, (47 + 10)),
                                dojo.date.locale.format(dataProximoContato, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })));
                        } else {
                            dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativaAux.replace("xx/xx/xxxx",
                                dojo.date.locale.format(dataProximoContato, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })))
                        }
                       
                    } else {
                        dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativa.replace("xx/xx/xxxx",
                        dojo.date.locale.format(dataProximoContato, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })))
                    }

                    if (hasValue(dojo.byId("cbTurma").value) && !Array.isArray(horario)) {
                        dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_inicial",
                            horario.dt_hora_ini));

                        dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_final",
                            horario.dt_hora_fim));
                        msgAulaDemonstrativaAux = dijit.byId("mensagemFollowUppartial").value;
                    } else if(hasValue(dojo.byId("cbTurma").value) && Array.isArray(horario))
                    {
                        dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_inicial",
                            horario[0].dt_hora_ini));

                        dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_final",
                            horario[0].dt_hora_fim));
                        msgAulaDemonstrativaAux = dijit.byId("mensagemFollowUppartial").value;
                    }
                    
                } else if (hasValue(horario) && horario.length > 1) {
                    //Verifica se a sequencia de horarios é contínua
                    var sequenciaContinua = horario.every(function (value, index) {
                        return !hasValue(horario[index + 1]) ||
                            (hasValue(horario[index + 1]) &&
			                value.dt_hora_fim ==
			                horario[index + 1].dt_hora_ini);
	                });

                    var dt_hora_ini_aux = "";
                    var dt_hora_fim_aux = "";

                    if (sequenciaContinua) {
	                    //Caso a turma tenha mais de 1 intervalo de horarios pega a minima hora inicial
	                     dt_hora_ini_aux = horario.map((value) => {
		                    return value.dt_hora_ini;
	                    }).sort()[0];
						 //Caso a turma tenha mais de 1 intervalo de horarios pega a maxima hora final
	                     dt_hora_fim_aux = horario.map((value) => {
		                    return value.dt_hora_fim;
	                    }).sort().reverse()[0];
                    } else {
                        //Se não for continua pega o primeiro intervalo
                        dt_hora_ini_aux = horario[0].dt_hora_ini;
                        dt_hora_fim_aux = horario[0].dt_hora_fim;
                    }
                    

	                apresentaMensagem('apresentadorMensagemCadFollowUpPartial', "");
	                if (hasValue(msgAulaDemonstrativaAux)) {
		                if (dijit.byId("mensagemFollowUppartial").value.indexOf("xx/xx/xxxx") == -1) {//se ja fez replace, acha a posicao da data 
			                dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativaAux.replace(msgAulaDemonstrativaAux.substring(47, (47 + 10)),
				                dojo.date.locale.format(dataProximoContato, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })));
		                } else {
			                dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativaAux.replace("xx/xx/xxxx",
				                dojo.date.locale.format(dataProximoContato, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })))
		                }

	                } else {
		                dijit.byId("mensagemFollowUppartial").set("value", msgAulaDemonstrativa.replace("xx/xx/xxxx",
			                dojo.date.locale.format(dataProximoContato, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" })))
	                }

	                if (hasValue(dojo.byId("cbTurma").value)) {
		                dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_inicial",
			                dt_hora_ini_aux));

		                dijit.byId("mensagemFollowUppartial").set("value", dijit.byId("mensagemFollowUppartial").value.replace("h_final",
			                dt_hora_fim_aux));
		                msgAulaDemonstrativaAux = dijit.byId("mensagemFollowUppartial").value;
	                }

                }else {
                    dijit.byId("dtaProxContatoFollowUpFK")._onChangeActive = false;
                    dijit.byId("dtaProxContatoFollowUpFK").reset();
                    dijit.byId("dtaProxContatoFollowUpFK")._onChangeActive = true;

                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Data do próximo contato não corresponde aos dias da semana da turma.");
                    apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
                }
                hideCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', "");
            });
    });
}

//function verificaDiaSemanaTurmaFollowUp() {
//    var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
//    if (hasValue(dojo.byId("cd_turma_pesquisa").value) && dojo.byId("cd_turma_pesquisa").value > 0
//        && hasValue(gridPesquisaTurmaFK) &&
//            hasValue(gridPesquisaTurmaFK.itensSelecionados[0].horarios)) {
//        var contemDiaSemana = jQuery.grep(gridPesquisaTurmaFK.itensSelecionados[0].horarios, function (horario) {
//            if (getDiaSemanaTurma() == horario.id_dia_semana)
//                return true;
//            return false;
//        });

//        if (hasValue(contemDiaSemana))
//            return true;
//        return false;
//    }
//    return true;
//}