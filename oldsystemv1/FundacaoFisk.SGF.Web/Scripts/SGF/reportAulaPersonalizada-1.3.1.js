//#region Monta relatório
function mascarar() {
    require([
           "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                $("#timeIni").mask("99:99");
                $("#timeFim").mask("99:99");
                $("#timeAulaIni").mask("99:99");
                $("#timeAulaFim").mask("99:99");
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function montarMetodosReportAulaPersonalizada() {
    require([
    "dojo/ready",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/on"
    ], function (ready, Memory, Button, JsonRest, ObjectStore, Cache, on) {
        ready(function () {
            try {
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFK();
                                });
                            }
                            else
                                abrirAlunoFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesAlunoFK");
                
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {  }
                }, "limparAlunoFK");

                var pesAlunoFK = dojo.byId('pesAlunoFK');
                if (hasValue(pesAlunoFK)) {
                    pesAlunoFK.parentNode.style.minWidth = '18px';
                    pesAlunoFK.parentNode.style.width = '18px';
                }

                if (hasValue(document.getElementById("limparAlunoFK"))) {
                    dojo.byId("limparAlunoFK").parentNode.style.minWidth = '40px';
                    dojo.byId("limparAlunoFK").parentNode.style.width = '40px';
                }

                sugereDataCorrente();

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFK();
                                });
                            }
                            else
                                abrirAlunoFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesAluno");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                            montarGridPesquisaTurmaFK(function () {
                                abrirTurmaFK();
                                dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                        montarGridPesquisaAluno(false, function () {
                                            abrirAlunoFKTurmaFK(true);
                                        });
                                    }
                                    else
                                        abrirAlunoFKTurmaFK(true);
                                });
                            });
                        else
                            abrirTurmaFK();
                    }
                }, "pesTurma");
                var pesAluno = document.getElementById('pesAluno');
                if (hasValue(pesAluno)) {
                    pesAluno.parentNode.style.minWidth = '18px';
                    pesAluno.parentNode.style.width = '18px';
                }
                var pesTurma = document.getElementById('pesTurma');
                if (hasValue(pesTurma)) {
                    pesTurma.parentNode.style.minWidth = '18px';
                    pesTurma.parentNode.style.width = '18px';
                }
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("cdAlunoPes").value = 0;
                        dojo.byId("noAluno").value = "";
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("limparAluno").set('disabled', true);

                        // Habilita e popula o produto:
                        dijit.byId('cbProduto').set('disabled', true);
                        dijit.byId('cbCurso').set('disabled', true);
                    }
                }, "limparAluno");
                if (hasValue(document.getElementById("limparAluno"))) {
                    document.getElementById("limparAluno").parentNode.style.minWidth = '40px';
                    document.getElementById("limparAluno").parentNode.style.width = '40px';
                }
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("noTurma").value = "";
                        dojo.byId("cdTurmaPes").value = 0;
                        dijit.byId("limparTurma").set('disabled', true);
                    }
                }, "limparTurma");
                if (hasValue(document.getElementById("limparTurma"))) {
                    document.getElementById("limparTurma").parentNode.style.minWidth = '40px';
                    document.getElementById("limparTurma").parentNode.style.width = '40px';
                }
                adicionarAtalhoPesquisa(['dtInicial', 'dtAulaIni', 'timeIni', 'timeAulaFim', 'timeAulaIni', 'timeAulaFim'], 'relatorio', ready);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm', '765px', '771px');
                        });
                }
            } catch (e) {
                postGerarLog(e);
            }
        });
        
    });
}

function testaPeriodo(dataInicial, dataFinal) {
    try{
        var retorno = true;
        if (dojo.date.compare(dataInicial, dataFinal) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            retorno = false;
        } else
            apresentaMensagem('apresentadorMensagem', "");
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion


function emitirRelatorio() {
    try {
        require(["dojo/window"], function (windowUtils) {
        
            if (!mostrarMensagemCampoValidado(windowUtils, dijit.byId('noAluno')))
                return false;

            var dataInicial = dojo.byId("dtInicial").value;
            var dataFinal = dojo.byId("dtFinal").value;
    
            if (!testaPeriodo(dijit.byId("dtInicial").value, dijit.byId("dtFinal").value))
                return false;

            var cd_aluno = dojo.byId('cdAlunoPes').value;
            var cd_produto = dijit.byId('cbProduto').value;
            var cd_curso = dijit.byId('cbCurso').value;
            var data_inicial_agend = dojo.byId('dtInicial').value;
            var data_final_agend = dojo.byId('dtFinal').value;
            var hora_inicial_agend = dojo.byId('timeIni').value;
            var hora_final_agend = dojo.byId('timeFim').value;
            var data_inicial_lanc = dojo.byId('dtAulaIni').value;
            var data_final_lanc = dojo.byId('dtAulaFim').value;
            var hora_inicial_lanc = dojo.byId('timeAulaIni').value;
            var hora_final_lanc = dojo.byId('timeAulaFim').value;

            dojo.xhr.get({
                url: Endereco() + "/api/coordenacao/getUrlRelatorioAulaPersonalizadaEspecializado?cd_aluno=" + cd_aluno + "&cd_produto=" + cd_produto + "&cd_curso=" + cd_curso
                                + "&dt_inicial_agend=" + data_inicial_agend + "&dt_final_agend=" + data_final_agend
                                + "&hr_inicial_agend=" + hora_inicial_agend + "&hr_final_agend=" + hora_final_agend + "&dt_inicial_lanc=" + data_inicial_lanc + "&dt_final_lanc=" + data_final_lanc
                                + "&hr_inicial_lanc=" + hora_inicial_lanc + "&hr_final_lanc=" + hora_final_lanc + "&no_aluno="+dojo.byId('noAluno').value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    abrePopUp(Endereco() + '/Relatorio/RelatorioAulaPersonalizada?' + data, '1024px', '750px', 'popRelatorio');
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = RELMATRICULA;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        pesquisaAlunoFKRel();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaAlunoFKRel() {
    try{
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        
        var myStore = dojo.store.Cache(
            dojo.store.JsonRest({
                target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                        document.getElementById("inicioAlunoFK").checked + "&origemFK=0" + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=0&opcao=0",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_aluno"
            }), dojo.store.Memory({ idProperty: "cd_aluno" }));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoRelFK() {
    try {
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);

            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("cdAlunoPes").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAluno').set("disabled", false);
        }
        if (!valido)
            return false;

        // Habilita e popula o produto:
        dijit.byId('cbProduto').set('disabled', false);
        dijit.byId('cbCurso').set('disabled', false);

        //Popula os produtos e cursos:
        populaProdutos('cbProduto', gridPesquisaAluno.itensSelecionados[0].cd_aluno);
        populaCurso('cbCurso', gridPesquisaAluno.itensSelecionados[0].cd_aluno);

        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKRelMatricula() {
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
                dojo.byId("noTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dojo.byId("cdTurmaPes").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dijit.byId('limparTurma').set("disabled", false);
            }
            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugereDataCorrente() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                dijit.byId('dtAulaIni').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", date);
                dijit.byId('dtAulaFim').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function populaProdutos(field, cd_aluno) {
    try {
        // Popula os produtos:
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/Coordenacao/getProdutoAulaPersonalizada?cd_aluno="+cd_aluno,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataProdAtivo) {
                loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, field, 0);
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadProduto(items, linkProduto, idProduto) {
    require(["dojo/store/Memory", "dojo/_base/array"],
	function (Memory, Array) {
	    try {
	        var itemsCb = [];
	        var cbProduto = dijit.byId(linkProduto);

	        Array.forEach(items, function (value, i) {
	            itemsCb.push({ id: value.cd_produto, name: value.no_produto });
	        });
	        var stateStore = new Memory({
	            data: itemsCb
	        });
	        cbProduto.store = stateStore;
	        if (hasValue(idProduto)) {
	            cbProduto._onChangeActive = false;
	            cbProduto.set("value", idProduto);
	            cbProduto._onChangeActive = true;
	        }
	    }
	    catch (e) {
	        postGerarLog(e);
	    }
	});
}

function populaCurso(field, cd_aluno) {
    var ativo = 1;
    dojo.xhr.get({
        url: Endereco() + "/api/curso/getCursosAulaPersonalizada?cd_aluno="+cd_aluno,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, field, 'cd_curso', 'no_curso');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}