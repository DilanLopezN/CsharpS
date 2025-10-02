var HAS_PRODUTO = 4;

//#region métodos auxiliares para o funcionamento do formulário  - mostraTabs - maskDate - maskHour - cleanUsarCpf - clearFormAtividade - limparFormAulaReposicao -  keepValues - loadCursoByProduto(cdProduto)

//Monta máscaras para telefone, cpf e cnpj
function mascarar() {
    require([
           "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                //maskDate("#dataAtividade");
                //maskDate("#dtaIni");
                //maskDate("#dtaFim");
                //$("#hrInicial").mask("99:99");
                //$("#hrFinal").mask("99:99");
                //$("#hrLimite").mask("99:99");
                $("#hhIni").mask("99:99");
                $("#hhFim").mask("99:99");
                $("#cbHoraIni").mask("99:99");
                $("#cbHoraFim").mask("99:99");
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function cleanUsarCpf() {
    try {
        dojo.byId("cdPessoaCpf").value = "";
        dojo.byId("nomPessoaCpf").value = "";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clearFormAtividade() {
    try {
        dijit.byId("cbSalaCad").reset();
        dijit.byId("cbResponsaveis").reset();
        dijit.byId("cbHoraIni").reset();
        dijit.byId("cbHoraFim").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function changeActive(ativa) {
    dijit.byId('cbHoraIni')._onChangeActive = ativa;
    dijit.byId('cbHoraFim')._onChangeActive = ativa;
    dijit.byId('dataAtividade')._onChangeActive = ativa;
    dijit.byId('cbResponsaveis')._onChangeActive = ativa;
}

function limparFormAulaReposicao(ready, Memory, filteringSelect, ObjectStore) {
    try {
        changeActive(false);
        dojo.byId('cd_aula_reposicao').value = 0;
        dijit.byId("cbResponsaveis").reset();
        dijit.byId("cbSalaCad").reset();
        dijit.byId("cbHoraIni").reset();
        dijit.byId("cbHoraFim").reset();
        dijit.byId('ckCarga').set('value', false);
        dijit.byId('ckPagar').set('value', false);
        //dijit.byId("hrInicial").reset();
        //dijit.byId("hrFinal").reset();
        dojo.byId("txObs").value = "";
        dojo.byId('turmaCad').value = '';
        dojo.byId('cdTurmaCad').value = null;
        dojo.byId('dtIniTurmaCad').value = null;
        dijit.byId('dataAtividade').set('value', new Date());

        if (dijit.byId("cbHoraIni").disabled == true) {
	        dijit.byId("cbHoraIni").set("disabled", false);
        }

        if (dijit.byId("cbHoraFim").disabled == true) {
            dijit.byId("cbHoraFim").set("disabled", false);
        }

        if (dijit.byId("cbSalaCad").disabled == true) {
            dijit.byId("cbSalaCad").set("disabled", false);
        }

        if (dijit.byId("cbResponsaveis").disabled == true) {
	        dijit.byId("cbResponsaveis").set("disabled", false);
        }

        dojo.byId('turmaDest').value = '';
        dojo.byId('cdTurmaDest').value = null;
        dojo.byId("cdProdutoTurmaCad").value = null;
        dojo.byId("cdCursoTurmaCad").value = null;
	    dojo.byId("cdEstagioTurmaCad").value = null;




        apresentaMensagem("apresentadorMensagemAulaReposicao", "");

        var gridAlunoAulaReposicao = dijit.byId("gridAlunoAulaReposicao");
        var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });

        gridAlunoAulaReposicao.setStore(dataStore);
        gridAlunoAulaReposicao.update();
        changeActive(true);

    }
    catch (e) {
        postGerarLog(e);
    }
}

//Pega os Antigos dados do Formulário
function keepValues(val, grid, ehLink) {
    try {
        var value = grid.itemSelecionado;
        var linkAnterior = document.getElementById('link');
        var gridAlunoAtv = dijit.byId('gridAlunoAulaReposicao');
        var Hora = ":00"
        var strlabel = "";
        var itemI = [];
        var itemF = [];

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;
        if (value != null && value.cd_aula_reposicao > 0) {

            changeActive(false);

            dojo.byId("cd_aula_reposicao").value = value.cd_aula_reposicao;
            //dijit.byId("hrInicial").value = value.dh_inicial_evento != null ? dojo.byId("hrInicial").value = value.dh_inicial_evento.replace(":00", '') : "";
            //dijit.byId("hrFinal").value = value.dh_final_evento != null ? dojo.byId("hrFinal").value = value.dh_final_evento.replace(":00", '') : "";
            dijit.byId("dataAtividade").value = value.dta_aula_reposicao != null ? dojo.byId("dataAtividade").value = value.dta_aula_reposicao : "";
            dojo.byId('cdUsuario').value = value.cd_atendente;

            if (hasValue(dijit.byId('cbResponsaveis'), true))
                hasValue(value.cd_professor) ? dijit.byId('cbResponsaveis').set("value", value.cd_professor) : 0;
            if (hasValue(dijit.byId('cbSalaCad'), true))
                hasValue(value.cd_sala) ? dijit.byId('cbSalaCad').set("value", value.cd_sala) : null;

            dijit.byId('ckCarga').set('value', value.id_carga_horaria);
            dijit.byId('ckPagar').set('value', value.id_pagar_professor);


            if (hasValue(value.tx_observacao_aula))
                dojo.byId("txObs").value = value.tx_observacao_aula;
            else dijit.byId("txObs").set('value', '');
             
            dojo.byId("cdTurmaCad").value = value.cd_turma;
            dojo.byId("dtIniTurmaCad").value = value.dtaIniAula;
            dojo.byId("cdSalaCad").value = value.cd_sala;
            dojo.byId("turmaCad").value = value.no_turma;

            if (value.cd_turma_destino > 0 && hasValue(value.no_turma_destino))  {
	            dojo.byId("cdTurmaDest").value = value.cd_turma_destino;
                dojo.byId("turmaDest").value = value.no_turma_destino;

                if (value.cd_produto > 0) {
	                dojo.byId('cdProdutoTurmaCad').value = value.cd_produto;
                }

                if (value.cd_curso > 0) {
	                dojo.byId('cdCursoTurmaCad').value = value.cd_curso;
                }

                if (value.cd_estagio > 0) {
	                dojo.byId('cdEstagioTurmaCad').value = value.cd_estagio;
                }

            }
            

            // método para carregar a pessoa logada no sistema
            // Podemos ter vários usuarios persitindo o formulário, no caso se o registro for editado, o usuario que alterar ou deletar o registro pode ser diferente do usuario que inseriu o registro, por isso dois campos de ID.
            dojo.byId('txAtendente').value = value.no_usuario;
            pesquisarAlunoAulaReposicao();
            dijit.byId("cbHoraIni").value = value.dh_inicial_evento != null ? dojo.byId("cbHoraIni").value = value.dh_inicial_evento.replace(":00", '') : "";
            dijit.byId("cbHoraFim").value = value.dh_final_evento != null ? dojo.byId("cbHoraFim").value = value.dh_final_evento.replace(":00", '') : "";
            //dojo.byId("cbHoraIni").value = value.dh_inicial_evento.replace(":00", '');
            //dojo.byId("cbHoraFim").value = value.dh_final_evento.replace(":00", '');
            //if (hasValue(dijit.byId('cbHoraIni'), true))
            //    hasValue(value.dh_inicial_evento) ? dijit.byId('cbHoraIni').set("value", value.dh_inicial_evento) : 0;
            //if (hasValue(dijit.byId('cbHoraFim'), true))
            //    hasValue(value.dh_final_evento) ? dijit.byId('cbHoraFim').set("value", value.dh_final_evento) : 0;
            if (hasValue(dijit.byId('cbHoraIni'), true) && hasValue(dojo.byId('cbHoraIni').value) && dijit.byId('cbHoraIni').value == dojo.byId('cbHoraIni').value) {
                Hora = dojo.byId('cbHoraIni').value + Hora
                if ((Hora.substring(3, 5) == "30") || (Hora.substring(3, 5) == "00")) strlabel = '<center><code> <b>' + dojo.byId('cbHoraIni').value + '</b></code> </center>';
                else strlabel = '<center><code>' + dojo.byId('cbHoraIni').value + '</code></center>';
                itemI.push({ id: Hora, name: dojo.byId('cbHoraIni').value, label: strlabel });
                dijit.byId('cbHoraIni').item = itemI;
            }
            if (hasValue(dijit.byId('cbHoraFim'), true) && hasValue(dojo.byId('cbHoraFim').value) && dijit.byId('cbHoraFim').value == dojo.byId('cbHoraFim').value) {
                Hora = dojo.byId('cbHoraFim').value + ":00"
                if ((Hora.substring(3, 5) == "30") || (Hora.substring(3, 5) == "00")) strlabel = '<center><code> <b>' + dojo.byId('cbHoraFim').value + '</b></code> </center>';
                else strlabel = '<center><code>' + dojo.byId('cbHoraFim').value + '</code></center>';
                itemF.push({ id: Hora, name: dojo.byId('cbHoraFim').value, label: strlabel });
                dijit.byId('cbHoraFim').item = itemF;
            }


            if (value.cd_turma_destino > 0 && hasValue(value.no_turma_destino)) {
	            var itemsSala = new Array({
		            cd_sala: value.cd_sala,
		            no_sala: value.no_sala
	            });
	            //preenche o store
	            loadSala(itemsSala, "cbSalaCad");

	            //seta a sala e desabilita o campo
	            dijit.byId("cbSalaCad")._onChangeActive = false;
	            dijit.byId("cbSalaCad").set("value", value.cd_sala);
	            dijit.byId("cbSalaCad")._onChangeActive = true;
                dijit.byId("cbSalaCad").set("disabled", true);

                //desabilita professor/horarios
                dijit.byId("cbResponsaveis").set("disabled", true);
                dijit.byId("cbHoraIni").set("disabled", true);
                dijit.byId("cbHoraFim").set("disabled", true);

            }


            changeActive(true);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


//#region formatação do checks das grids
function formatCheckBoxAulaReposicao(value, rowIndex, obj) {
    try {
        var gridName = 'gridAulaReposicao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAulaReposicao');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aula_reposicao", grid._by_idx[rowIndex].item.cd_aula_reposicao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aula_reposicao', 'selecionadoAulaReposicao', -1, 'selecionaTodosAulaReposicao', 'selecionaTodosAulaReposicao', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aula_reposicao', 'selecionadoAulaReposicao', " + rowIndex + ", '" + id + "', 'selecionaTodosAulaReposicao', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAlunoAulaReposicao(value, rowIndex, obj) {
    try {
        var gridName = 'gridAlunoAulaReposicao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAlunoAulaReposicao');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aluno", grid._by_idx[rowIndex].item.cd_aluno);
            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aluno', 'selecionadoAlunoAulaReposicao', -1, 'selecionaTodosAlunoAulaReposicao', 'selecionaTodosAlunoAulaReposicao', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aluno', 'selecionadoAlunoAulaReposicao', " + rowIndex + ", '" + id + "', 'selecionaTodosAlunoAulaReposicao', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxParticipacao(value, rowIndex, obj) {
    try {
        var gridName = 'gridAlunoAulaReposicao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input disabled='disabled' class='formatCheckBox' id='" + id + "'/> ";

        setTimeout("configuraCheckBoxParticipou(" + value + ", '" + id + "', '" + gridName + "', 'disabled')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxParticipou(value, id, gridName, disabled) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            try {
                var dojoId = dojo.byId(id);
                var grid = dijit.byId(gridName);


                if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                    dijit.byId(id).destroy();
                if (value == undefined)
                    value = false;
                if (disabled == null || disabled == undefined) disabled = false;
                if (hasValue(dojoId) && dojoId.type == 'text')
                    var checkBox = new dijit.form.CheckBox({
                        name: "checkBox",
                        checked: value,
                        disabled: disabled
                    }, id);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}
//#endregion

//#region montar os dropsdown
//function getReturnDataAulaReposicao(fieldProfessor, fieldAluno) {
//    var aulaReposicaoPesq = new AulaReposicaoPesquisa(null);
//    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
//        xhr.post(Endereco() + "/coordenacao/returnDataAulaReposicao", {
//            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
//            handleAs: "json",
//            data: ref.toJson(aulaReposicaoPesq)
//        }).then(function (dataAulaReposicao) {
//            try {
//                if (!hasValue(dataAulaReposicao.erro)) {
//                    loadProfessor(dataAulaReposicao.retorno.professores, fieldProfessor);
//                    loadSala(dataAulaReposicao.retorno.salas, fieldAluno);
//                    // método para carregar a pessoa logada no sistema
//                    // Podemos ter vários usuarios persitindo o formulário, no caso se o registro for editado, o usuario que alterar ou deletar o registro pode ser diferente do usuario que inseriu o registro, por isso dois campos de ID.
//                    dojo.byId('txAtendente').value = dataAulaReposicao.retorno.no_usuario;
//                    dojo.byId('cdUsuario').value = dataAulaReposicao.retorno.cd_usuario_atendente;
//                } else
//                    apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
//            }
//            catch (e) {
//                postGerarLog(e);
//            }
//        },
//        function (error) {
//            apresentaMensagem('apresentadorMensagem', error.response.data);
//        });
//    });
//}

function getReturnDataAulaReposicao(dataAulaReposicao, fieldProfessor, fieldAluno) {
    var aulaReposicaoPesq = new AulaReposicaoPesquisa(null);
    try {
        if (!hasValue(dataAulaReposicao.erro)) {
            loadProfessor(dataAulaReposicao.retorno.professoresEdit, fieldProfessor);
            //loadSala(dataAulaReposicao.retorno.salas, fieldAluno);
            fieldAluno != null ? loadSala(dataAulaReposicao.retorno.salas, fieldAluno) : "";
            // método para carregar a pessoa logada no sistema
            // Podemos ter vários usuarios persitindo o formulário, no caso se o registro for editado, o usuario que alterar ou deletar o registro pode ser diferente do usuario que inseriu o registro, por isso dois campos de ID.
            dojo.byId('txAtendente').value = dataAulaReposicao.retorno.no_usuario;
            dojo.byId('cdUsuario').value = dataAulaReposicao.retorno.cd_atendente;
        } else
            apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Método para pesquisa
function returnDataAulaReposicaoParaPesquisa(fieldProfessor, fieldAluno) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/returnDataAulaReposicaoParaPesquisa", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(null)
        }).then(function (dataAulaReposicao) {
            try {
                dataAulaReposicao = jQuery.parseJSON(dataAulaReposicao);
                if (!hasValue(dataAulaReposicao.erro)) {
                    loadProfessor(dataAulaReposicao.retorno.professores, fieldProfessor);
                    loadSala(dataAulaReposicao.retorno.salas, fieldAluno);
                    //loadAluno(dataAulaReposicao.retorno.alunos, fieldAluno);
                } else
                    apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {

            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgLockPermissionSearchFilter);

            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            //apresentaMensagem('apresentadorMensagem', error.response.data);
        });
    });

}

// fIm dos métodos do Curso

function loadProfessor(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbResponsavel = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_pessoa, name: value.no_pessoa });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbResponsavel.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function populaSala(field, horaIni, horaFim, data) {
    try {
        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
        data = hasValue(dojo.byId("dataAtividade").value) ? dojo.byId("dataAtividade").value : 0;
        horaIni = hasValue(dojo.byId("cbHoraIni").value) ? dojo.byId("cbHoraIni").value : null;
        horaFim = hasValue(dojo.byId("cbHoraFim").value) ? dojo.byId("cbHoraFim").value : null;
        var idAtividadeExtra = hasValue(dojo.byId('cd_aula_reposicao').value > 0) ? dojo.byId('cd_aula_reposicao').value : null;
        var cdTurma = dojo.byId("cdTurmaCad").value;
//        if (!dijit.byId("cbHoraFim").validate() || !dijit.byId("cbHoraIni").validate() || !dijit.byId("dataAtividade").validate())

        if (!dijit.byId("dataAtividade").validate())
            return false;
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/getSalaHorariosDisponiveisAulaRep?horaIni=" + horaIni + "&horaFim=" + horaFim + "&data=" + data + "&cd_aula_reposicao=" + idAtividadeExtra + "&cd_turma=" + cdTurma,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataSala) {
                try {
                    var dados = $.parseJSON(dataSala);
                    if (!hasValue(dados.erro) && dados.retorno.length > 0 && dados.MensagensWeb.length == 0) {
                        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                        loadSala(dados.retorno, field);
                    } else {
                        var mensagensWeb = new Array();
                        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, dados.MensagensWeb[0].mensagem);
                        apresentaMensagem("apresentadorMensagemAulaReposicao", mensagensWeb);
                        dijit.byId("cbSalaCad").reset();
                        loadSala(dados.retorno, field);
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemAulaReposicao', error.response.data);
                clearFormAtividade();
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSala(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbResponsavel = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_sala, name: value.no_sala });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbResponsavel.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function populaHorario(field, turma, professor, alunos) {
    try {
        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
        turma = hasValue(dojo.byId("cdTurmaCad").value) ? dojo.byId("cdTurmaCad").value : 0;
        professor = hasValue(dijit.byId("cbResponsaveis").value) ? dijit.byId("cbResponsaveis").value : 0;
        //alunos = cloneArray(gridAlunosPAulaReposicao.store.objectStore.data;;
        var gridAlunos = dijit.byId("gridAlunoAulaReposicao");
        if (hasValue(gridAlunos) && gridAlunos.listaCompletaAlunos.length > 0)
            alunos = gridAlunos.listaCompletaAlunos;
        else {
            alunos = [];
            return false;
        };

        if (turma == 0 || professor == 0 || (hasValue(alunos) && alunos.length == 0))
            return false;
        data = hasValue(dojo.byId("dataAtividade").value) ? dojo.byId("dataAtividade").value : 0;
        if (!dijit.byId("dataAtividade").validate())
            return false;
        var idAulaReposicao = hasValue(dojo.byId('cd_aula_reposicao').value > 0) ? dojo.byId('cd_aula_reposicao').value : null;
            require(["dojo/_base/xhr" , "dojox/json/ref"], function (xhr, ref) {
                xhr.post({
                url: Endereco() + "/api/coordenacao/postHorariosDisponiveisAulaRep?data=" + data + "&turma=" + turma + "&professor=" + professor + "&cdAulaReposicao=" + idAulaReposicao,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(alunos)
            }).then(function (dataSala) {
                try {
                    var dados = $.parseJSON(dataSala);
                    if (!hasValue(dados.erro) && dados.retorno.length > 0 && dados.MensagensWeb.length == 0) {
                        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                        loadHorario(dados.retorno, 'cbHoraIni', 'cbHoraFim');
                    } else {
                        var mensagensWeb = new Array();
                        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, dados.MensagensWeb[0].mensagem);
                        apresentaMensagem("apresentadorMensagemAulaReposicao", mensagensWeb);
                        dijit.byId("cbHoraIni").reset();
                        dijit.byId("cbHoraFim").reset();
                        loadHorario(dados.retorno, 'cbHoraIni','cbHoraFim');
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemAulaReposicao', error.response.data);
                clearFormAtividade();
            });
        });
    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function loadHorario(items, fieldIni, fieldFim) {
    require(["dojo/store/Memory", "dojo/_base/array", "dojo/date/locale"],
    function (Memory, Array, locale) {
        try {
            var strlabel = "";
            var itemsCb = [];
            var cbHoraIni = dijit.byId(fieldIni);
            var cbHoraFim = dijit.byId(fieldFim);
            cbHoraIni.labelAttr = "label";
            cbHoraIni.labelType = "html";
            cbHoraFim.labelAttr = "label";
            cbHoraFim.labelType = "html";
            Array.forEach(items, function (value, i) {
                var Hora = value.substring(0,5);
                if ((value.substring(3, 5) == "30") || (value.substring(3, 5) == "00")) strlabel = '<center><code> <b>' + Hora + '</b></code> </center>';
                else strlabel = '<center><code>' + Hora + '</code></center>';
                itemsCb.push({ id: value, name: Hora, label: strlabel });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbHoraIni.store = stateStore;
            cbHoraFim.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadHorarioGenerico(items, field) {
	require(["dojo/store/Memory", "dojo/_base/array", "dojo/date/locale"],
		function (Memory, Array, locale) {
			try {
				var strlabel = "";
				var itemsCb = [];
                var cbHora = dijit.byId(field);
				cbHora.labelAttr = "label";
				cbHora.labelType = "html";
				Array.forEach(items, function (value, i) {
					var Hora = value.substring(0, 5);
					if ((value.substring(3, 5) == "30") || (value.substring(3, 5) == "00")) strlabel = '<center><code> <b>' + Hora + '</b></code> </center>';
					else strlabel = '<center><code>' + Hora + '</code></center>';
					itemsCb.push({ id: value, name: Hora, label: strlabel });
				});
				var stateStore = new Memory({
					data: itemsCb
				});
				cbHora.store = stateStore;
			}
			catch (e) {
				postGerarLog(e);
			}
		});
}

function verificaHorarios(horaIni, horaFim, data) {
    try {
        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
        data = hasValue(dojo.byId("dataAtividade").value) ? dojo.byId("dataAtividade").value : 0;
        horaIni = hasValue(dojo.byId("cbHoraIni").value) ? dojo.byId("cbHoraIni").value : null;
        horaFim = hasValue(dojo.byId("cbHoraFim").value) ? dojo.byId("cbHoraFim").value : null;
        var idAtividadeExtra = hasValue(dojo.byId('cd_aula_reposicao').value > 0) ? dojo.byId('cd_aula_reposicao').value : null;
        var cdTurma = dojo.byId("cdTurmaCad").value;
        var cdProfessor = hasValue(dijit.byId("cbResponsaveis").value) ? dijit.byId("cbResponsaveis").value : null;
        var gridAlunos = dijit.byId("gridAlunoAulaReposicao");
        if (hasValue(gridAlunos) && gridAlunos.listaCompletaAlunos.length > 0)
            alunos = gridAlunos.listaCompletaAlunos;
        else {
            alunos = [];
            return false;
        };

        if (!dijit.byId("cbHoraFim").validate() || !dijit.byId("cbHoraIni").validate() || !dijit.byId("dataAtividade").validate() || cdTurma == null || cdProfessor == null || (hasValue(alunos) && alunos.length == 0))
            return false;
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/coordenacao/postVerificaHorarioAulaRep?horaIni=" + horaIni + "&horaFim=" + horaFim + "&data=" + data + "&cd_aula_reposicao=" + idAtividadeExtra + "&cd_turma=" + cdTurma + "&cd_professor=" + cdProfessor, 
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(alunos)
        }).then(function (dataHorario) {
                try {
                    var dados = $.parseJSON(dataHorario);
                    if (!hasValue(dados.erro) && dados.retorno == 0 && dados.MensagensWeb.length == 0) {
                        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                    } else {
                        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, dados.MensagensWeb[0].mensagem);
                        apresentaMensagem("apresentadorMensagemAulaReposicao", mensagensWeb);
                        changeActive(false);
                        dijit.byId("cbHoraIni").set("value", null);
                        dijit.byId("cbHoraFim").set("value", null);
                        changeActive(true);
                        caixaDialogo(DIALOGO_AVISO, (dados.MensagensWeb[0].mensagem.length > 85) ? dados.MensagensWeb[0].mensagem.substring(0, 85) : dados.MensagensWeb[0].mensagem, null);
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                caixaDialogo(DIALOGO_AVISO, (error.response.data.length > 85) ? error.response.data.substring(0, 85) : error.response.data, null);
                clearFormAtividade();
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region  Criação da Grades e métodos auxiliares - manipulaDropDownSala - montarObjetoAulaReposicao
function montarCadastroAulaReposicao() {
    require([
        "dojo/_base/xhr",
        "dojox/grid/EnhancedGrid",
        "dojo/data/ItemFileReadStore",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/form/FilteringSelect",
        "dojo/date",
        "dojo/on",
        "dojo/require",
        "dijit/Dialog",
        "dojo/parser",
        "dojo/domReady!"
    ], function (xhr, EnhancedGrid, ItemFileReadStore, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, filteringSelect, date) {
        ready(function () {
            try {
                mascarar();
                var registroGrid = null;
                var desabilitaCamposLancamento = false;
                returnDataAulaReposicaoParaPesquisa('cbResponsavel', 'cbSala');

                var parametros = getParamterosURL();
                var cdProf = null;
                xhr.get({
                    url: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data);
                        //if (eval(parametros['lancamento']))
                        //    if (data.retorno != null && data.retorno.Professor != null && data.retorno.Professor.cd_pessoa > 0 && !data.retorno.Professor.id_coordenador) {
                        //        dojo.byId("cbResponsavel").value = data.retorno.Professor.no_fantasia;
                        //        dijit.byId("cbResponsavel").set('value', data.retorno.Professor.cd_pessoa);
                        //        dijit.byId("cbResponsavel").set('cd_pessoa', data.retorno.Professor.cd_pessoa);
                        //        dijit.byId("cbResponsavel").set("disabled", true);
                        //        cdProf = data.retorno.Professor.cd_pessoa;
                        //        if (eval(Master()))
                        //            cdProf = null;
                        //        else
                        //            desabilitaCamposLancamento = true;
                        //    }
                        //    else if (!eval(Master()) && (data.retorno == null || !data.retorno.Professor.id_coordenador)) {
                        //        var mensagensWeb = new Array();
                        //        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'O usuário que está lançando esta atividade não está vinculado a um Responsável.');
                        //        apresentaMensagem('apresentadorMensagem', mensagensWeb);

                        //        desabilitaCamposLancamento = true;
                        //        cdProf = 0;
                        //    }
                        var myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/coordenacao/getAulaReposicaoSearch?dataIni=" + minDate + "&dataFim=" + maxDate + "&hrInicial=null&hrFinal=null&cd_turma=null&cd_aluno=null&cd_responsavel=null&cd_sala=null",
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                idProperty: "cd_aula_reposicao"
                            }
                        ), Memory({ idProperty: "cd_aula_reposicao" }));

                        var gridAulaReposicao = new EnhancedGrid({
                            //store: ObjectStore({ objectStore: myStore }),
                            store: ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure: [
                                { name: "<input id='selecionaTodosAulaReposicao' style='display:none'/>", field: "selecionadoAulaReposicao", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAulaReposicao },
                                { name: "Data", field: "dta_aula_reposicao", width: "8%", styles: "text-align: left; min-width:75px; max-width:75px;" },
                                { name: "Hr. Inicial", field: "dh_inicial_evento", width: "8%", styles: "text-align: center;" },
                                { name: "Hr. Final", field: "dh_final_evento", width: "8%", styles: "text-align: center;" },
                                { name: "Turma", field: "no_turma", width: "30%", styles: "text-align: left;" },
                                { name: "Sala", field: "no_sala", width: "15%", styles: "text-align: left;" },
                                { name: "Responsável", field: "no_responsavel", width: "20%", styles: "text-align: left;" },
                                { name: "Alunos", field: "nm_alunos", width: "8%", styles: "text-align: center;" }
                            ],
                            noDataMessage: msgNotRegEncFiltro,
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
                        }, "gridAulaReposicao"); // make sure you have a target HTML element with this id
                        // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                        gridAulaReposicao.pagination.plugin._paginator.plugin.connect(gridAulaReposicao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            verificaMostrarTodos(evt, gridAulaReposicao, 'cd_aula_reposicao', 'selecionaTodosAulaReposicao');
                        });
                        gridAulaReposicao.startup();
                        gridAulaReposicao.canSort = function (col) { return Math.abs(col) != 1; };
                        gridAulaReposicao.on("RowDblClick", function (evt) {
                            try {
                                var idx = evt.rowIndex,
                                      item = this.getItem(idx),
                                      store = this.store;
                                registroGrid = item
                                //showCarregando();
                                apresentaMensagem('apresentadorMensagem', '');
                                gridAulaReposicao.itemSelecionado = montarObjetoAulaReposicao(item);
                                var existIdSala = gridAulaReposicao.itemSelecionado.cd_sala ? true : false;
                                aulaReposicaoPesq = new AulaReposicaoPesquisa(item);
                                dijit.byId("cbSalaCad").store.data = null;

                                require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
                                    xhr.post(Endereco() + "/coordenacao/getAulaReposicaoViewOnDbClik", {
                                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                        handleAs: "json",
                                        data: ref.toJson(aulaReposicaoPesq)
                                    }).then(function (dataAulaReposicao) {
                                        try {
                                            showCarregando();
                                            if (!hasValue(dataAulaReposicao.erro)) {
                                                limparFormAulaReposicao(ready, Memory, filteringSelect, ObjectStore);
                                                obterRecursosAulaReposicaoDbClick(dataAulaReposicao, gridAulaReposicao);
                                            } else
                                                apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
                                        }
                                        catch (er) {
                                            postGerarLog(er);
                                        }
                                    },
                                    function (error) {
                                        showCarregando();
                                        apresentaMensagem('apresentadorMensagem', error);
                                    });
                                });
                                dijit.byId("txAtendente").set("disabled", true);
                                IncluirAlterar(0, 'divAlterarAulaReposicao', 'divIncluirAulaReposicao', 'divExcluirAulaReposicao', 'apresentadorMensagemAulaReposicao', 'divCancelarAulaReposicao', 'divLimparAulaReposicao');
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }, true);

                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(gridAulaReposicao, "_onFetchComplete", function () {
                                // Configura o check de todos:
                                if (dojo.byId('selecionaTodosAulaReposicao').type == 'text')
                                    setTimeout("configuraCheckBox(false, 'cd_aula_reposicao', 'selecionadoAulaReposicao', -1, 'selecionaTodosAulaReposicao', 'selecionaTodosAulaReposicao', 'gridAulaReposicao')", gridAulaReposicao.rowsPerPage * 3);
                            });
                        });

                        //***************** Adiciona link de ações:****************************\\
                        var menu = new DropDownMenu({ style: "height: 25px" });
                        var acaoEditar = new MenuItem({
                            label: "Editar",
                            onClick: function () {
                                eventoEditarAulaReposicao(dijit.byId('gridAulaReposicao').itensSelecionados, ready, Memory, filteringSelect, ObjectStore);
                            }
                        });
                        menu.addChild(acaoEditar);

                        var acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () { eventoRemover(gridAulaReposicao.itensSelecionados, 'DeletarAulaReposicao(itensSelecionados)'); }
                        });
                        menu.addChild(acaoRemover);

                        var button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasAulaReposicao",
                            dropDown: menu,
                            id: "acoesRelacionadasAulaReposicao"
                        });
                        dom.byId("linkAcoesAulaReposicao").appendChild(button.domNode);

                        // Adiciona link de selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });
                        var menuTodosItens = new MenuItem({
                            label: "Todos Itens",
                            onClick: function () { buscarTodosItens(gridAulaReposicao, 'todosItensAulaReposicao', ['pesquisarAulaReposicao', 'relatorioAulaReposicao']); pesquisarAulaReposicao(false); }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () { buscarItensSelecionados('gridAulaReposicao', 'selecionadoAulaReposicao', 'cd_aula_reposicao', 'selecionaTodosAulaReposicao', ['pesquisarAulaReposicao', 'relatorioAulaReposicao'], 'todosItensAulaReposicao'); }
                        });
                        menu.addChild(menuItensSelecionados);

                        var button = new DropDownButton({
                            label: "Todos Itens",
                            name: "todosItensAulaReposicao",
                            dropDown: menu,
                            id: "todosItensAulaReposicao"
                        });
                        dom.byId("linkSelecionadosAulaReposicao").appendChild(button.domNode);

                        //*** Cria os botões de persistência **\\

                        new Button({
                            label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                            onClick: function () {
                                IncluirAulaReposicao();
                            }
                        }, "incluirAulaReposicao");
                        new Button({
                            label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                            onClick: function () {
                                AlteraAulaReposicao();
                            }
                        }, "alterarAulaReposicao");
                        new Button({
                            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                            onClick: function () {
                                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                                    DeletarAulaReposicao();
                                });
                            }
                        }, "deleteAulaReposicao");
                        new Button({
                            label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                            onClick: function () {
                                limparFormAulaReposicao(ready, Memory, filteringSelect, ObjectStore);
                            }
                        }, "limparAulaReposicao");
                        new Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                            onClick: function () {
                                try {
                                    apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                                    changeActive(false);
                                    //dijit.byId('hrInicial')._onChangeActive = false;
                                    //dijit.byId('hrFinal')._onChangeActive = false;
                                    //dijit.byId('dataAtividade')._onChangeActive = false;
                                    //dijit.byId('cbResponsaveis')._onChangeActive = false;
                                    aulaReposicaoPesq = new AulaReposicaoPesquisa(registroGrid);
                                    showCarregando();
                                    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
                                        xhr.post(Endereco() + "/coordenacao/getAulaReposicaoViewOnDbClik", {
                                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                            handleAs: "json",
                                            data: ref.toJson(aulaReposicaoPesq)
                                        }).then(function (dataAulaReposicao) {
                                            if (!hasValue(dataAulaReposicao.erro)) {

                                                obterRecursosAulaReposicaoClickCancel(dataAulaReposicao, gridAulaReposicao);
                                                //aulaReposicaoPesq,
                                                //    dataAulaReposicao.retorno.cursos,
                                                //    gridAulaReposicao);

                                            } else {
                                                apresentaMensagem('apresentadorMensagemAulaReposicao', data);
                                            }
                                            showCarregando();
                                        },
                                        function (error) {
                                            showCarregando();
                                            apresentaMensagem('apresentadorMensagemAulaReposicao', error.response.data);
                                        });
                                    });
                                    changeActive(true);
                                    //dijit.byId('hrInicial')._onChangeActive = true;
                                    //dijit.byId('hrFinal')._onChangeActive = true;
                                    //dijit.byId('dataAtividade')._onChangeActive = true;
                                    //dijit.byId('cbResponsaveis')._onChangeActive = true;
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cancelarAulaReposicao");
                        new Button({
                            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                            onClick: function () {
                                dijit.byId("cadAulaReposicao").hide();
                            }
                        }, "fecharAulaReposicao");
                        new Button({
                            label: "",
                            iconClass: 'dijitEditorIconSearchSGF',
                            onClick: function () {
                                apresentaMensagem('apresentadorMensagem', null);
                                pesquisarAulaReposicao(true);
                            }
                        }, "pesquisarAulaReposicao");
                        decreaseBtn(document.getElementById("pesquisarAulaReposicao"), '32px');

                        //Botões do dialogo aluno/atividades
                        new Button({
                            label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                            onClick: function () {
                                alterarAvaliacaoGrid(dijit.byId("gridAlunoAulaReposicao"));
                                dijit.byId("dialogAluno").hide();
                            }
                        }, "incluirAluno");
                        new Button({
                            label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                            type: "reset",
                            onClick: function () {
                                document.getElementById("descObs").value = "";
                                dojo.byId("peso").checked = false;
                            }
                        }, "limparAluno");
                        new Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                            onClick: function () {
                                keepValuesAluno(dijit.byId("gridAlunoAulaReposicao"));
                            }
                        }, "cancelarAluno");
                        new Button({
                            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                            onClick: function () {
                                dijit.byId("dialogAluno").hide();
                            }
                        }, "fecharAluno");

                        new Button({
                            label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                            onClick: function () {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFkCad();
                                    });
                                }
                                else
                                    abrirAlunoFkCad();
                            }
                        }, "incluirAlunoFK");

                        if (eval(parametros['lancamento']))
                            dijit.byId("incluirAlunoFK").set('disabled', true);
                        else {
                            dijit.byId("ckParticipouAluno").set('disabled', true);
                            dijit.byId("descObs").set('disabled', false);
                        }
                        //fim

                        new Button({
                            label: "Novo",
                            iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                            onClick: function () {
                                //Limpa a grade
                                showCarregando();
                                var gridAlunoAulaReposicao = dijit.byId("gridAlunoAulaReposicao");
                                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                                xhr.get({
                                    url: Endereco() + "/coordenacao/returnDataAulaReposicao",
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (dataTipoAulaReposicao) {
                                    try {
                                        changeActive(true);
                                        //dijit.byId('hrInicial')._onChangeActive = true;
                                        //dijit.byId('hrFinal')._onChangeActive = true;
                                        //dijit.byId('dataAtividade')._onChangeActive = true;
                                        //dijit.byId('cbResponsaveis')._onChangeActive = true;
                                        dijit.byId("cbSalaCad").store.data = null;
                                        dijit.byId("cbHoraIni").store.data = null;
                                        dijit.byId("cbHoraFim").store.data = null;

                                        limparFormAulaReposicao(ready, Memory, filteringSelect, ObjectStore);
                                        dijit.byId("tagAlunos").set("open", false);
                                        apresentaMensagem('apresentadorMensagem', null);
                                        dijit.byId('txAtendente').set('disabled', true);
                                        IncluirAlterar(1, 'divAlterarAulaReposicao', 'divIncluirAulaReposicao', 'divExcluirAulaReposicao', 'apresentadorMensagemAulaReposicao', 'divCancelarAulaReposicao', 'divLimparAulaReposicao');
                                        getReturnDataAulaReposicao(dataTipoAulaReposicao,"cbResponsaveis", null);
                                        dojo.addOnLoad(function () {
                                            dijit.byId('dataAtividade').attr("value", new Date(ano, mes, dia));
                                        });
                                        //if (hasValue(dijit.byId("gridAlunoAulaReposicao"))) {
                                        //    dijit.byId("gridAlunoAulaReposicao").update();
                                        //}

                                        gridAlunoAulaReposicao.setStore(dataStore);
                                        gridAlunoAulaReposicao.update();
                                        gridAlunoAulaReposicao.listaCompletaAlunos = [];
                                        if (hasValue(dataTipoAulaReposicao) && hasValue(dataTipoAulaReposicao.retorno)) {
                                            if (hasValue(dataTipoAulaReposicao.retorno.professoresEdit))
                                                loadProfessor(dataTipoAulaReposicao.retorno.professoresEdit, "cbResponsaveis");
                                            //if (hasValue(dataTipoAulaReposicao.retorno.salasDisponiveis))
                                            //    loadSala(dataTipoAulaReposicao.retorno.salasDisponiveis, "cbSalaCad");
                                            dojo.byId('txAtendente').value = dataTipoAulaReposicao.retorno.no_usuario;
                                            dojo.byId('cdUsuario').value = dataTipoAulaReposicao.retorno.cd_atendente;
                                        }
                                        dijit.byId("cadAulaReposicao").show();
                                    }
                                    catch (e) {
                                        postGerarLog(e);
                                    }
                                    showCarregando();
                                },
                                function (error) {
                                    showCarregando();
                                    apresentaMensagem('apresentadorMensagemAulaReposicao', error);
                                });
                            }
                        }, "novaAulaReposicao");

                        //----Monta o botão de Relatório----
                        var variaveisRelatorio = new VariaveisPesquisa();
                        new Button({
                            label: getNomeLabelRelatorio(),
                            iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                            onClick: function () {
                                try {
                                    if (!hasValue(gridAulaReposicao.itensSelecionados) || gridAulaReposicao.itensSelecionados.length < 1) {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Selecione alguma aula de reposição para emitir o relatório.');
                                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                    }
                                    else
                                        if (gridAulaReposicao.itensSelecionados.length > 1) {
                                            var mensagensWeb = new Array();
                                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Selecione somente uma aula de reposição para emitir o relatório.');
                                            apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                        }
                                        else {
                                            apresentaMensagem('apresentadorMensagem', null);
                                            var cdAulaReposicao = gridAulaReposicao.itensSelecionados[0].cd_aula_reposicao;

                                            require(["dojo/_base/xhr"], function (xhr) {
                                                xhr.get({
                                                    url: Endereco() + "/api/coordenacao/GetUrlRelatorioAulaReposicao?cdAulaReposicao=" + cdAulaReposicao,
                                                    preventCache: true,
                                                    handleAs: "json",
                                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                                }).then(function (data) {
                                                    abrePopUp(Endereco() + '/Relatorio/RelatorioAulaReposicaoView?' + data, '765px', '771px', 'popRelatorio');
                                                },
                                                function (error) {
                                                    apresentaMensagem('apresentadorMensagem', error);
                                                });
                                            })
                                        }
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "relatorioAulaReposicao");

                        // botões de pesquisa de aluno
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFk();
                                    });
                                }
                                else
                                    abrirAlunoFk();
                            }
                        }, "pesAluno");

                        new Button({
                            label: "limpar", iconClass: '', type: "reset",
                            onClick: function () {
                                dojo.byId('alunoPesq').value = '';
                                dojo.byId('cdAlunoPesq').value = null;
                            }
                        }, "limparAlunoPesc");

                        if (hasValue(document.getElementById("limparAlunoPesc"))) {
                            document.getElementById("limparAlunoPesc").parentNode.style.minWidth = '40px';
                            document.getElementById("limparAlunoPesc").parentNode.style.width = '40px';
                        }

                        // botões de pesquisa de turma
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                    montarGridPesquisaTurmaFK(function () {
                                        abrirTurmaFK();
                                        dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                                montarGridPesquisaAluno(false, function () {
                                                    abrirAlunoFKTurmaFK(true,AULAREPOSICAO);
                                                });
                                            }
                                            else
                                                abrirAlunoFKTurmaFK(true,AULAREPOSICAO);
                                        });
                                    });
                                else
                                    abrirTurmaFK();
                            }
                        }, "pesTurma");

                        new Button({
                            label: "limpar", iconClass: '', type: "reset",
                            onClick: function () {
                                dojo.byId('turmaPesq').value = '';
                                dojo.byId('cdTurmaPesq').value = null;
                            }
                        }, "limparTurmaPesc");

                        if (hasValue(document.getElementById("limparTurmaPesc"))) {
                            document.getElementById("limparTurmaPesc").parentNode.style.minWidth = '40px';
                            document.getElementById("limparTurmaPesc").parentNode.style.width = '40px';
                        }

                        /* Turma Cad*/
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                    montarGridPesquisaTurmaFK(function () {
                                        abrirTurmaCadFK();
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
                                    abrirTurmaCadFK();
                            }
                        }, "cadTurma");

                        new Button({
                            label: "limpar", iconClass: '',
                            onClick: function () {
                                dojo.byId('turmaCad').value = '';
                                dojo.byId('cdTurmaCad').value = null;
                                dojo.byId('cdProdutoTurmaCad').value = null;
                                dojo.byId('cdCursoTurmaCad').value = null;
                                dojo.byId('cdEstagioTurmaCad').value = null;
                                dojo.byId('dtIniTurmaCad').value = null;

                                dojo.byId('turmaDest').value = '';
                                dojo.byId('cdTurmaDest').value = null;
                                dojo.byId('dtIniTurmaDest').value = null;



                                if (dijit.byId("cbHoraIni").disabled == true) {
                                    dijit.byId("cbHoraIni").set("disabled", false);
                                    dijit.byId("cbHoraIni").reset();
	                                dijit.byId("cbHoraIni").store.data = null;
                                }

                                if (dijit.byId("cbHoraFim").disabled == true) {
                                    dijit.byId("cbHoraFim").set("disabled", false);
                                    dijit.byId("cbHoraFim").reset();
	                                dijit.byId("cbHoraFim").store.data = null;
                                }

                                if (dijit.byId("cbSalaCad").disabled == true) {
                                    dijit.byId("cbSalaCad").set("disabled", false);
                                    dijit.byId("cbSalaCad").reset();
	                                dijit.byId("cbSalaCad").store.data = null;
                                }

                                if (dijit.byId("cbResponsaveis").disabled == true) {
                                    dijit.byId("cbResponsaveis").set("disabled", false);
                                    dijit.byId("cbResponsaveis").reset();
	                                dijit.byId("cbResponsaveis").store.data = null;
                                }

                                dojo.byId("cdProdutoTurmaCad").value = null;
                                dojo.byId("cdCursoTurmaCad").value = null;
                                dojo.byId("cdEstagioTurmaCad").value = null;
                            }
                        }, "limparTurmaCad");

                        if (hasValue(document.getElementById("limparTurmaCad"))) {
                            document.getElementById("limparTurmaCad").parentNode.style.minWidth = '40px';
                            document.getElementById("limparTurmaCad").parentNode.style.width = '40px';
                        }

                        

                        /* Turma Destino*/
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                if (!hasValue(dojo.byId("turmaCad").value)) {
	                                var mensagensWeb = new Array();
	                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Para inserir uma turma de destino é necessário inserir uma turma de primeiro.');
                                    apresentaMensagem('apresentadorMensagemAulaReposicao', mensagensWeb);
                                    return false;
                                }
                                if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                    montarGridPesquisaTurmaFK(function () {
                                        debugger
	                                    abrirTurmaCadDestFK();
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
	                                abrirTurmaCadDestFK();
                            }
                        }, "cadTurmaDest");

                        new Button({
                            label: "limpar", iconClass: '',
                            onClick: function () {

                                dojo.byId('turmaDest').value = '';
                                dojo.byId('cdTurmaDest').value = null;
                                dojo.byId('dtIniTurmaDest').value = null;



                                if (dijit.byId("cbHoraIni").disabled == true) {
	                                dijit.byId("cbHoraIni").set("disabled", false);
	                                dijit.byId("cbHoraIni").reset();
	                                dijit.byId("cbHoraIni").store.data = null;
                                }

                                if (dijit.byId("cbHoraFim").disabled == true) {
	                                dijit.byId("cbHoraFim").set("disabled", false);
	                                dijit.byId("cbHoraFim").reset();
	                                dijit.byId("cbHoraFim").store.data = null;
                                }

                                if (dijit.byId("cbSalaCad").disabled == true) {
	                                dijit.byId("cbSalaCad").set("disabled", false);
	                                dijit.byId("cbSalaCad").reset();
	                                dijit.byId("cbSalaCad").store.data = null;
                                }

                                if (dijit.byId("cbResponsaveis").disabled == true) {
	                                dijit.byId("cbResponsaveis").set("disabled", false);
	                                dijit.byId("cbResponsaveis").reset();
	                                dijit.byId("cbResponsaveis").store.data = null;
                                }



                            }
                        }, "limparTurmaDest");

                        if (hasValue(document.getElementById("limparTurmaDest"))) {
                            document.getElementById("limparTurmaDest").parentNode.style.minWidth = '40px';
                            document.getElementById("limparTurmaDest").parentNode.style.width = '40px';
                        }

                        //Altera o tamanho dos botões
                        var buttonFkArray = ['pesAluno', 'pesTurma', 'cadTurma', 'cadTurmaDest'];
                        diminuirBotoes(buttonFkArray);
                        //Grid de aluno atividade
                        var data = new Array();
                        var gridAlunoAulaReposicao = new EnhancedGrid({
                            store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data, idProperty: "cd_aluno_aula_reposicao" }) }),
                            structure:
                              [
                                { name: "<input id='selecionaTodosAlunoAulaReposicao' style='display:none'/>", field: "selecionadoAlunoAulaReposicao", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAlunoAulaReposicao },
                                //{ name: "Código", field: "cd_aluno", width: "15%", minwidth: "10%" },
                                { name: "Nome", field: "no_pessoa", width: "35%", minwidth: "10%" },
                                { name: "Participou", field: "id_participacao", width: "20%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxParticipacao },
                                { name: "Obs.", field: "tx_observacao_aluno_aula", width: "50%", minwidth: "10%" }
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
                                    /*page step to be displayed*/
                                    maxPageStep: 5,
                                    /*position of the pagination bar*/
                                    position: "button",
                                    plugins: { nestedSorting: true }
                                }
                            }
                        }, "gridAlunoAulaReposicao");;
                        gridAlunoAulaReposicao.canSort = function (col) { return Math.abs(col) != 1; };
                        // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                        gridAlunoAulaReposicao.pagination.plugin._paginator.plugin.connect(gridAlunoAulaReposicao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            verificaMostrarTodos(evt, gridAlunoAulaReposicao, 'cd_aluno_aula_reposicao', 'selecionaTodosAlunoAulaReposicao');
                        });

                        gridAlunoAulaReposicao.startup();
                        gridAlunoAulaReposicao.listaCompletaAlunos = [];
                        gridAlunoAulaReposicao.itensSelecionados = [];
                        gridAlunoAulaReposicao.on("RowDblClick", function (evt) {
                            try {
                                dijit.byId("dialogAluno").show();
                                var idx = evt.rowIndex,
                                   item = this.getItem(idx),
                                   store = this.store;
                                document.getElementById("cd_aluno").value = item.cd_aluno,
                                document.getElementById("aluno").value = item.no_pessoa;
                                dijit.byId("ckParticipouAluno").set("value", item.ind_participacao);
                                document.getElementById("descObs").value = item.tx_observacao_aluno_aula;
                                document.getElementById('divCancelarAluno').style.display = "";
                                document.getElementById('divLimparAluno').style.display = "none";
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }, true);

                        // Adiciona link de selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });

                        var acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () {
                                deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAlunoAulaReposicao"));
                            }
                        });

                        var acaoMarcaParticipou = new MenuItem({
                            label: "Participou",
                            onClick: function () {
                                ParticipouNao(true);
                            }
                        });

                        var acaoMarcaNaoParticipou = new MenuItem({
                            label: "Não Participou",
                            onClick: function () {
                                ParticipouNao(false);
                            }
                        });

                        if (hasValue(dijit.byId("acoesRelacionadasAlunoAulaReposicao"))) {
                            dijit.byId("acoesRelacionadasAlunoAulaReposicao").destroy();
                            $('linkAcoesAlunoAulaReposicao').attr('id', 'gridAlunoAulaReposicao').appendTo('#contentAlunoAulaReposicao');
                        }
                        var button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasAlunoAulaReposicao",
                            dropDown: menu,
                            id: "acoesRelacionadasAlunoAulaReposicao"
                        });
                        menu.addChild(acaoMarcaParticipou);
                        menu.addChild(acaoMarcaNaoParticipou);
                        if (!eval(parametros['lancamento']))
                            menu.addChild(acaoRemover);
                        dom.byId("linkAcoesAlunoAulaReposicao").appendChild(button.domNode);

                        //fim da grade

                        dijit.byId("cbHoraIni").on("click", function (hrIni) {
                            if (dijit.byId("cbHoraIni").store.data==null) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Deverão ser informados primeiramente a Turma, Professor e Alunos para a escolha de horários');
                                apresentaMensagem('apresentadorMensagemAulaReposicao', mensagensWeb);
                            }
                        });

                        dijit.byId("cbHoraFim").on("click", function (hrFim) {
                            if (dijit.byId("cbHoraFim").store.data == null) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Deverão ser informados primeiramente a Turma, Professor e Alunos para a escolha de horários');
                                apresentaMensagem('apresentadorMensagemAulaReposicao', mensagensWeb);
                            }
                        });

                        dijit.byId("cbSalaCad").on("click", function (e) {
                            if (dijit.byId("cbSalaCad").store.data == null) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Deverão ser informados primeiramente a Data e horários inicial e final para a escolha de salas');
                                apresentaMensagem('apresentadorMensagemAulaReposicao', mensagensWeb);
                            }
                        });

                        dijit.byId("cbHoraIni").on("change", function (hrIni) {
                            try {
                                if (!validarHoraView('cbHoraIni', 'cbHoraFim', 'apresentadorMensagemAulaReposicao')) {
                                    dijit.byId('cbHoraIni').set('value', null);
                                    return;
                                }

                                var data = hasValue(dojo.byId('dataAtividade').value) ? dojo.byId('dataAtividade').value : 0;
                                var horaFinal = hasValue(dojo.byId('cbHoraFim').value) ? dojo.byId('cbHoraFim').value : 0;
                                var horaInicial = hasValue(hrIni) ? hrIni : 0;
                                manipulaDropDownSala(horaInicial, horaFinal, data);
                                manipulaHorario(horaInicial, horaFinal, data);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("cbHoraFim").on("change", function (hrFinal) {
                            try {
                                if (!validarHoraView('cbHoraIni', 'cbHoraFim', 'apresentadorMensagemAulaReposicao')) {
                                    dijit.byId('cbHoraFim').set('value', null);
                                    return;
                                }

                                var data = hasValue(dojo.byId('dataAtividade').value) ? dojo.byId('dataAtividade').value : 0;
                                var horaInicial = hasValue(dojo.byId("cbHoraIni").value) ? dojo.byId("cbHoraIni").value : 0;
                                var horaFinal = hasValue(hrFinal) ? hrFinal : 0;
                                manipulaDropDownSala(horaInicial, horaFinal, data);
                                manipulaHorario(horaInicial, horaFinal, data);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("dataAtividade").on("change", function (dta) {
                            try {
                                var dtaInicio = null;
                                var dtaFinal = null;
                                if (hasValue(dta))
                                    dtaInicio = hasValue(dta) ? dta : null;
                                if (hasValue(dojo.byId('dtIniTurmaCad').value))
                                    dtaFinal = hasValue(dojo.byId('dtIniTurmaCad').value) ? dojo.date.locale.parse(dojo.byId('dtIniTurmaCad').value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

                                //Verifica se a data inicial é maior que a data final:
                                if (dtaInicio != null & dtaFinal != null)
                                    if (dta < dtaFinal) {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgHorarioAulaReposicao);
                                        apresentaMensagem('apresentadorMensagemAulaReposicao', mensagensWeb);
                                        dijit.byId('dataAtividade').set('value', null);
                                        return;
                                    }
                                    else
                                        apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                                var data = hasValue(dta)? dta : 0;
                                var horaInicial = hasValue(dojo.byId("cbHoraIni").value) ? dojo.byId("cbHoraIni").value : 0;
                                var horaFinal = hasValue(dojo.byId('cbHoraFim').value) ? dojo.byId('cbHoraFim').value : 0;
                                manipulaDropDownSala(horaInicial, horaFinal, data);
                                manipulaHorario(horaInicial, horaFinal, data);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("cbResponsaveis").on("change", function () {
                            try {
                                populaHorario();
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("tagAlunos").on("show", function (e) {
                            dijit.byId("gridAlunoAulaReposicao").update();
                            dijit.byId("contentAlunoAulaReposicao").resize();
                        });
                        if (hasValue(dijit.byId("menuManual"))) {
                            if (hasValue(dijit.byId("menuManual"))) {
                                dijit.byId("menuManual").on("click",
                                    function (e) {
                                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323056',
                                            '765px',
                                            '771px');
                                    });
                            }
                        }
                        adicionarAtalhoPesquisa(['cbResponsavel', 'dtaIni', 'hhIni', 'dtaFim', 'hhFim'], 'pesquisarAulaReposicao', ready);

                        showCarregando();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, function (exception) {
                    showCarregando();

                    var myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                idProperty: "cd_aula_reposicao"
                            }
                        ), Memory({ idProperty: "cd_aula_reposicao" }));

                    var gridAulaReposicao = new EnhancedGrid({
                        store: ObjectStore({ objectStore: myStore }),
                        structure: [
                            { name: "<input id='selecionaTodosAulaReposicao' style='display:none'/>", field: "selecionadoAulaReposicao", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAulaReposicao },
                            { name: "Data", field: "dta_atividade_extra", width: "10%", styles: "text-align: left; min-width:75px; max-width:75px;" },
                            { name: "Hr. Inicial", field: "hh_inicial", width: "10%", styles: "text-align: center;" },
                            { name: "Hr. Final", field: "hh_final", width: "10%", styles: "text-align: center;" },
                            { name: "Tipo Atividade", field: "no_tipo_atividade_extra", width: "10%", styles: "text-align: left;" },
                            { name: "Produto", field: "no_produto", width: "10%", styles: "text-align: left;" },
                            { name: "Curso", field: "no_curso", width: "10%", styles: "text-align: left;" },
                            { name: "Responsável", field: "no_responsavel", width: "20%", styles: "text-align: left;" },
                            { name: "Vagas", field: "nm_vagas", width: "10%", styles: "text-align: center;" },
                            { name: "Alunos", field: "nm_alunos", width: "10%", styles: "text-align: center;" }
                        ],
                        noDataMessage: "Nenhum registro encontrado.",
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
                    }, "gridAulaReposicao");
                    gridAulaReposicao.startup();

                    new Button({
                        label: "Novo",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF'
                    }, "novaAulaReposicao");

                    //----Monta o botão de Relatório----
                    new Button({
                        label: getNomeLabelRelatorio(),
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage'
                    }, "relatorioAulaReposicao");

                    new Button({
                        label: "",
                        iconClass: 'dijitEditorIconSearchSGF'
                    }, "pesquisarAulaReposicao");
                    decreaseBtn(document.getElementById("pesquisarAulaReposicao"), '32px');

                    // botões de pesquisa de aluno
                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                    }, "pesAluno");

                    new Button({
                        label: "limpar", iconClass: '', type: "reset"
                    }, "limparAlunoPesc");

                    if (hasValue(document.getElementById("limparAlunoPesc"))) {
                        document.getElementById("limparAlunoPesc").parentNode.style.minWidth = '40px';
                        document.getElementById("limparAlunoPesc").parentNode.style.width = '40px';
                    }


                    // botões de pesquisa de turma
                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                    }, "pesTurma");

                    new Button({
                        label: "limpar", iconClass: '', type: "reset"
                    }, "limparTurmaPesc");

                    if (hasValue(document.getElementById("limparTurmaPesc"))) {
                        document.getElementById("limparTurmaPesc").parentNode.style.minWidth = '40px';
                        document.getElementById("limparTurmaPesc").parentNode.style.width = '40px';
                    }

                    // botões de cad de turma
                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                    }, "cadTurma");

                    new Button({
                        label: "limpar", iconClass: '', type: "reset"
                    }, "limparTurmaCad");

                    if (hasValue(document.getElementById("limparTurmaCad"))) {
                        document.getElementById("limparTurmaCad").parentNode.style.minWidth = '40px';
                        document.getElementById("limparTurmaCad").parentNode.style.width = '40px';
                    }


                    //Altera o tamanho dos botões
                    var buttonFkArray = ['pesAluno', 'pesTurma', 'cadTurma'];
                    diminuirBotoes(buttonFkArray);

                });
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}
// Fim da criação das grades

function manipulaDropDownSala(horaInicial, horaFinal, data) {
    try {
        if (horaInicial == 0 || horaFinal == 0 || data == 0) {
            dijit.byId("cbSalaCad").set("disabled", true);
            dijit.byId("cbSalaCad").reset();
            dijit.byId("cbSalaCad").store.data = null;
        }
        else {
            dijit.byId("cbSalaCad").reset();
            dijit.byId("cbSalaCad").set("disabled", false);
            populaSala("cbSalaCad", horaInicial, horaFinal, data);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function manipulaHorario(horaInicial, horaFinal, data) {
    try {
        if (horaInicial != 0 && horaFinal != 0 && data != 0) {
            verificaHorarios(horaInicial, horaFinal, data);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarObjetoAulaReposicao(item) {
    try {

	    var objeto =
	    {
		    cd_aula_reposicao: item.cd_aula_reposicao,
		    cd_pessoa_escola: item.cd_pessoa_escola,
		    cd_atendente: item.cd_atendente,
		    cd_professor: item.cd_professor,
		    dta_aula_reposicao: item.dta_aula_reposicao,
		    dh_inicial_evento: item.dh_inicial_evento,
		    dh_final_evento: item.dh_final_evento,
		    id_carga_horaria: item.id_carga_horaria,
		    id_pagar_professor: item.id_pagar_professor,
		    tx_observacao_aula: item.tx_observacao_aula,
		    cd_turma: item.cd_turma,
		    no_turma: item.no_turma,
		    no_responsavel: item.no_responsavel,
		    nm_alunos: item.nm_alunos,
		    alunoAulaReposicao: item.alunoAulaReposicao,
		    no_usuario: item.no_usuario,
		    cd_sala: item.cd_sala,
		    cd_turma_destino: item.cd_turma_destino,
		    no_turma_destino: item.no_turma_destino,
		    no_sala: item.no_sala,
		    cd_produto: item.cd_produto,
            cd_curso: item.cd_curso,
            cd_estagio: item.cd_estagio
    }
        return objeto;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function AulaReposicaoPesquisa(item) {
    try {
        if (item != null) {
            this.cd_aula_reposicao = item.cd_aula_reposicao;
            this.cd_pessoa_escola = item.cd_pessoa_escola;
            this.cd_atendente = item.cd_atendente;
            this.cd_professor = item.cd_professor;
            this.dta_aula_reposicao = item.dta_aula_reposicao;
            this.dh_inicial_evento = item.dh_inicial_evento;
            this.dh_final_evento = item.dh_final_evento;
            this.id_carga_horaria = item.id_carga_horaria;
            this.id_pagar_professor = item.id_pagar_professor;
            this.tx_observacao_aula = item.tx_observacao_aula;
            this.cd_turma = item.cd_turma;
            this.dt_aula_reposicao = item.dt_aula_reposicao;
            this.no_turma = item.no_turma;
            this.no_responsavel = item.no_responsavel;
            this.nm_alunos = item.nm_alunos;
            this.cd_sala = item.cd_sala;
            this.cd_turma_origem = item.cd_turma_origem;
            this.no_turma_origem = item.no_turma_origem;
        } else {
            cd_aula_reposicao = 0;
            cd_pessoa_escola = 0;
            cd_atendente = 0;
            cd_professor = 0;
            dta_aula_reposicao = "";
            dt_aula_reposicao = "";
            dh_inicial_evento = "";
            dh_final_evento = "";
            id_carga_horária = false;
            id_pagar_professor = false;
            tx_observacao_aula = "";
            cd_turma = 0;
            no_turma = "";
            no_responsavel = "";
            cd_aluno = 0;
            nm_alunos = 0;
            cd_turma_origem = 0;
            no_turma_origem = 0;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region Persistêcia da atividade extra

//Método de pesquisa

function pesquisarAulaReposicao(limparItens) {
    if (!validarHoraView('hhIni', 'hhFim', 'apresentadorMensagem'))
        return false;

    if (!validarDatasIniFim('dtaIni', 'dtaFim', 'apresentadorMensagem', dojo.date))
        return false;

    var variaveisPesquisa = new VariaveisPesquisa();
    variaveisPesquisa.hroFim = variaveisPesquisa.hroFim == '' ? null : variaveisPesquisa.hroFim;
    variaveisPesquisa.hroIni = variaveisPesquisa.hroIni == '' ? null : variaveisPesquisa.hroIni;

    if (!dijit.byId('hhIni').validate() || !dijit.byId('hhFim').validate() ||
       !dijit.byId('dtaIni').validate() || !dijit.byId('dtaFim').validate())
        return false;
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/coordenacao/getAulaReposicaoSearch?dataIni=" + variaveisPesquisa.dtaIni + "&dataFim=" + variaveisPesquisa.dtaFim + "&hrInicial=" + variaveisPesquisa.hroIni + "&hrFinal=" + variaveisPesquisa.hroFim + "&cd_turma=" + variaveisPesquisa.turmaSelected + "&cd_aluno=" + variaveisPesquisa.aluno + "&cd_responsavel=" + variaveisPesquisa.responsavel + "&cd_sala=" + variaveisPesquisa.sala,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_aula_reposicao"
                }
                ), Memory({ idProperty: "cd_aula_reposicao" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAulaReposicao = dijit.byId("gridAulaReposicao");
            if (limparItens) {
                gridAulaReposicao.itensSelecionados = [];
            }
            gridAulaReposicao.noDataMessage = msgNotRegEnc;
            gridAulaReposicao.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function VariaveisPesquisa() {
    try {
        if (hasValue(dojo.byId("dtaIni").value)) {
            this.dtaIni = dojo.byId("dtaIni").value;
            // this.dtaIni = dateFormat(dtaIni.value, 'dd/mm/yyyy');
        } else {
            this.dtaIni = minDate;
        }
        if (hasValue(dojo.byId("dtaFim").value)) {
            this.dtaFim = dojo.byId("dtaFim").value;
            //  this.dtaFim = dateFormat(dtaFim.value, 'dd/mm/yyyy');
        } else {
            this.dtaFim = maxDate;
        }
        if (hasValue(dojo.byId("hhIni").value))
            this.hroIni = dojo.byId("hhIni").value;
        else
            this.hroIni = '';
        if (hasValue(dojo.byId("hhFim").value))
            this.hroFim = dojo.byId("hhFim").value;
        else
            this.hroFim = '';

        this.responsavel = hasValue(dijit.byId("cbResponsavel").cd_pessoa) ? dijit.byId("cbResponsavel").cd_pessoa : (hasValue(dijit.byId("cbResponsavel").value) ? dijit.byId("cbResponsavel").value : null);
        this.sala = hasValue(dijit.byId("cbSala").cd_sala) ? dijit.byId("cbSala").cd_sala : (hasValue(dijit.byId("cbSala").value) ? dijit.byId("cbSala").value : null);
        this.aluno = hasValue(dojo.byId("cdAlunoPesq").value) ? dojo.byId("cdAlunoPesq").value : null;

        this.turmaSelected = hasValue(dojo.byId("cdTurmaPesq").value) ? dojo.byId("cdTurmaPesq").value : null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//IncluirItemServico
function IncluirAulaReposicao() {
    require([
     "dojo/date",
     "dojo/ready"
    ], function (date, ready) {
        ready(function () {
            try {
                apresentaMensagem('apresentadorMensagem', null);
                apresentaMensagem('apresentadorMensagemAulaReposicao', null);
                if (hasValue(dojo.byId('cdUsuario').value)) {
                    var cdUsuario = dojo.byId('cdUsuario').value;
                } else {
                    caixaDialogo(DIALOGO_AVISO, 'Usúario não identificado.', null);
                    return false;
                }
                if (dojo.byId("cbSalaCad").value == '') {
                    caixaDialogo(DIALOGO_AVISO, 'Sala não informada.', null);
                    return false;
                }

                var alunoAulaReposicao = montarAlunoAulaReposicao();
                var dataAtividade = hasValue(dojo.byId("dataAtividade").value) ? dojo.date.locale.parse(dojo.byId("dataAtividade").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

                var dataMin = new Date(1899, 12, 01);
                var dataMax = new Date(2079, 05, 06);
                if (date.compare(dataMin, dataAtividade) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtaMin);
                    apresentaMensagem("apresentadorMensagemAulaReposicao", mensagensWeb);
                    return false;
                } else if (date.compare(dataAtividade, dataMax) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtaMax);
                    apresentaMensagem("apresentadorMensagemAulaReposicao", mensagensWeb);
                    return false;
                }

                if (!dijit.byId("formAulaReposicao").validate()) return false;
                require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                    showCarregando();
                    xhr.post(Endereco() + "/api/coordenacao/postIncluirAulaReposicao", {
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        handleAs: "json",
                        data: ref.toJson({
                            dt_aula_reposicao: dataAtividade,
                            dh_final_evento: dijit.byId("cbHoraFim").value,
                            dh_inicial_evento: dijit.byId("cbHoraIni").value,
                            id_carga_horaria: domAttr.get("ckCarga", "checked"),
                            id_pagar_professor: domAttr.get("ckPagar", "checked"),
                            tx_observacao_aula: dom.byId("txObs").value,
                            cd_atendente: cdUsuario,
                            no_usuario: hasValue(dojo.byId('txAtendente').value) ? dojo.byId('txAtendente').value : "",
                            cd_turma: dojo.byId("cdTurmaCad").value,
                            no_turma: dojo.byId("turmaCad").value,
                            cd_turma_destino: hasValue(dojo.byId("cdTurmaDest").value) ? dojo.byId("cdTurmaDest").value : null,
                            no_turma_destino: hasValue(dojo.byId("turmaDest").value) ? dojo.byId("turmaDest").value : null,
                            cd_sala: dijit.byId("cbSalaCad").value,
                            no_sala: dojo.byId("cbSalaCad").value,
                            cd_professor: dijit.byId("cbResponsaveis").value,
                            no_responsavel: dojo.byId("cbResponsaveis").value,
                            AlunoAulaReposicao: alunoAulaReposicao
                        })
                    }).then(function (data) {
                        try {
                            showCarregando();
                            data = jQuery.parseJSON(data);
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridAulaReposicao';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                dijit.byId("cadAulaReposicao").hide();
                                if (!hasValue(grid.itensSelecionados)) {
                                    grid.itensSelecionados = [];
                                }
                                insertObjSort(grid.itensSelecionados, "cd_aula_reposicao", itemAlterado);
                                buscarItensSelecionados(gridName, 'selecionadoAulaReposicao', 'cd_aula_reposicao', 'selecionaTodosAulaReposicao', ['pesquisarAulaReposicao', 'relatorioAulaReposicao'], 'todosItensAulaReposicao');
                                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_aula_reposicao");
                            } else
                                apresentaMensagem('apresentadorMensagemAulaReposicao', data);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemAulaReposicao', error.response.data);
                    });
                });
            }
            catch (er) {
                postGerarLog(er);
            }
        });
    });
}

function AlteraAulaReposicao() {
    require([
        "dojo/date",
        "dojo/ready"
    ], function (date, ready) {
        ready(function () {
            try {
                var escola = jQuery.parseJSON(Escola());
                //var Hora = ":00"
                //var strlabel = "";
                //var itemI = [];
                //var itemF = [];
                apresentaMensagem('apresentadorMensagem', null);
                apresentaMensagem('apresentadorMensagemItemServico', null);
                if (hasValue(dojo.byId('cdUsuario').value)) {
                    var cdUsuario = dojo.byId('cdUsuario').value;
                } else {
                    caixaDialogo(DIALOGO_AVISO, 'Usuário não identificado.', null);
                    return false;
                }
                if (dojo.byId("cbSalaCad").value == '') {
                    caixaDialogo(DIALOGO_AVISO, 'Sala não informada.', null);
                    return false;
                }
                //if (hasValue(dijit.byId('cbHoraIni'), true) && hasValue(dojo.byId('cbHoraIni').value) && dijit.byId('cbHoraIni').value == dojo.byId('cbHoraIni').value) {
                //    Hora = dojo.byId('cbHoraIni').value + Hora
                //    if ((Hora.substring(3, 5) == "30") || (Hora.substring(3, 5) == "00")) strlabel = '<center><code> <b>' + dojo.byId('cbHoraIni').value + '</b></code> </center>';
                //    else strlabel = '<center><code>' + dojo.byId('cbHoraIni').value + '</code></center>';
                //    itemI.push({ id: Hora, name: dojo.byId('cbHoraIni').value, label: strlabel });
                //    dijit.byId('cbHoraIni').item = itemI;
                //}
                //if (hasValue(dijit.byId('cbHoraFim'), true) && hasValue(dojo.byId('cbHoraFim').value) && dijit.byId('cbHoraFim').value == dojo.byId('cbHoraFim').value) {
                //    Hora = dojo.byId('cbHoraFim').value + ":00"
                //    if ((Hora.substring(3, 5) == "30") || (Hora.substring(3, 5) == "00")) strlabel = '<center><code> <b>' + dojo.byId('cbHoraFim').value + '</b></code> </center>';
                //    else strlabel = '<center><code>' + dojo.byId('cbHoraFim').value + '</code></center>';
                //    itemF.push({ id: Hora, name: dojo.byId('cbHoraFim').value, label: strlabel });
                //    dijit.byId('cbHoraFim').item = itemF;
                //}

                var codAulaReposicao = dojo.byId("cd_aula_reposicao").value;
                var dataAtividade = hasValue(dojo.byId("dataAtividade").value) ? dojo.date.locale.parse(dojo.byId("dataAtividade").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                if (!dijit.byId("formAulaReposicao").validate()) return false;
                var alunoAulaReposicao = montarAlunoAulaReposicao();

                var dataMin = new Date(1899, 12, 01);
                var dataMax = new Date(2079, 05, 06);
                if (date.compare(dataMin, dataAtividade) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtaMin);
                    apresentaMensagem("apresentadorMensagemAulaReposicao", mensagensWeb);
                    return false;
                } else if (date.compare(dataAtividade, dataMax) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAulaReposicao", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtaMax);
                    apresentaMensagem("apresentadorMensagemAulaReposicao", mensagensWeb);
                    return false;
                }

                require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                    showCarregando();
                    xhr.post(Endereco() + "/api/coordenacao/postAlterarAulaReposicao", {
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        handleAs: "json",
                        data: ref.toJson({
                            cd_aula_reposicao: codAulaReposicao,
                            dt_aula_reposicao: dataAtividade,
                            dh_final_evento: dijit.byId("cbHoraFim").value,
                            dh_inicial_evento: dijit.byId("cbHoraIni").value,
                            id_carga_horaria: domAttr.get("ckCarga", "checked"),
                            id_pagar_professor: domAttr.get("ckPagar", "checked"),
                            tx_observacao_aula: dom.byId("txObs").value,
                            cd_atendente: cdUsuario,
                            no_usuario: hasValue(dojo.byId('txAtendente').value) ? dojo.byId('txAtendente').value : "",
                            cd_turma: dojo.byId("cdTurmaCad").value,
                            no_turma: dojo.byId("turmaCad").value,
                            cd_turma_destino: hasValue(dojo.byId("cdTurmaDest").value) ? dojo.byId("cdTurmaDest").value : null,
                            no_turma_destino: hasValue(dojo.byId("turmaDest").value) ? dojo.byId("turmaDest").value : null,
                            cd_professor: dijit.byId("cbResponsaveis").value,
                            no_responsavel: dojo.byId("cbResponsaveis").value,
                            cd_sala: dijit.byId('cbSalaCad').value,
                            no_sala: dojo.byId('cbSalaCad').value,
                            AlunoAulaReposicao: alunoAulaReposicao
 
                        })
                    }).then(function (data) {
                        try {
                            showCarregando();
                            data = jQuery.parseJSON(data);
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridAulaReposicao';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                dijit.byId("cadAulaReposicao").hide();
                                if (!hasValue(grid.itensSelecionados)) {
                                    grid.itensSelecionados = [];
                                }
                                removeObjSort(grid.itensSelecionados, "cd_aula_reposicao", dom.byId("cd_aula_reposicao").value);
                                insertObjSort(grid.itensSelecionados, "cd_aula_reposicao", itemAlterado);
                                buscarItensSelecionados(gridName, 'selecionadoAulaReposicao', 'cd_aula_reposicao', 'selecionaTodosAulaReposicao', ['pesquisarAulaReposicao', 'relatorioAulaReposicao'], 'todosItensAulaReposicao');
                                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_aula_reposicao");
                            } else
                                apresentaMensagem('apresentadorMensagemAulaReposicao', data);
                        }
                        catch (er) {
                            hideCarregando();
                            postGerarLog(er);
                        }
                    },
                    function (error) {
                        hideCarregando();
                        apresentaMensagem('apresentadorMensagemAulaReposicao', error.response.data);
                    });
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

// Deletar Atividade Extra
function DeletarAulaReposicao(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_aula_reposicao').value != 0)
                itensSelecionados = [{
                    cd_aula_reposicao: dom.byId("cd_aula_reposicao").value,
                    dt_aula_reposicao: hasValue(dojo.byId("dataAtividade").value) ? dojo.date.locale.parse(dojo.byId("dataAtividade").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : nul
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/PostDeleteAulaReposicao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensAulaReposicao");
                apresentaMensagem('apresentadorMensagem', data);
                data = jQuery.parseJSON(data).retorno;
                dijit.byId("cadAulaReposicao").hide();
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridAulaReposicao').itensSelecionados, "cd_aula_reposicao", itensSelecionados[r].cd_aula_reposicao);
                pesquisarAulaReposicao(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarAulaReposicao").set('disabled', false);
                dijit.byId("relatorioAulaReposicao").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadAulaReposicao").style.display))
                apresentaMensagem('apresentadorMensagemAulaReposicao', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

function pesquisarAlunoAulaReposicao() {
    var cdAlunoAulaReposicao = hasValue(dojo.byId('cd_aula_reposicao').value) ? dojo.byId('cd_aula_reposicao').value : 0;
    require([
          "dojo/_base/xhr",
          "dojo/data/ObjectStore",
          "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/GetAlunoAulaReposicao?cd_aula_reposicao=" + cdAlunoAulaReposicao,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_aluno_aula_reposicao"
        }).then(function (data) {
            try {
                showCarregando();
                var grid = dijit.byId("gridAlunoAulaReposicao");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: jQuery.parseJSON(data).retorno, idProperty: "cd_aluno_aula_reposicao" }) });
                grid.setStore(dataStore);
                quickSortObj(grid.itensSelecionados, 'cd_aluno_aula_reposicao');
                grid.listaCompletaAlunos = cloneArray(grid.store.objectStore.data);
                dijit.byId('tagAlunos').set('open', true);
                dijit.byId("gridAlunoAulaReposicao").update();
                showCarregando();
                populaHorario();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            hideCarregando();
            apresentaMensagem('apresentadorMensagemAulaReposicao', error.response.data);
        });
    });
}
//#endregion fim dos métodos de percistência

//#region Métodos auxiliares -  eventoEditarAulaReposicao -  alterarAvaliacaoGrid - keepValuesAluno -  montarAlunoAulaReposicao
function eventoEditarAulaReposicao(itensSelecionados, ready, Memory, filteringSelect, ObjectStore) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            aulaReposicaoPesq = new AulaReposicaoPesquisa(itensSelecionados[0]);

            require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
                xhr.post(Endereco() + "/coordenacao/getAulaReposicaoViewOnDbClik", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson(aulaReposicaoPesq)
                }).then(function (dataAulaReposicao) {
                    try {
                        showCarregando();
                        if (!hasValue(dataAulaReposicao.erro)) {
                            limparFormAulaReposicao(ready, Memory, filteringSelect, ObjectStore);
                            changeActive(false);
                            obterRecursosAulaReposicao(aulaReposicaoPesq, itensSelecionados);
                            changeActive(true);
                            //loadSala(dataAulaReposicao.retorno.salasDisponiveis, "cbSalaCad");

                            //dojo.byId('cdUsuario').value = dataAulaReposicao.retorno.cd_usuario_atendente;

                            //getLimpar('#formAulaReposicao');
                            //apresentaMensagem('apresentadorMensagem', '');
                            //dijit.byId('gridAulaReposicao').itemSelecionado = itensSelecionados[0];
                            //keepValues(null, dijit.byId('gridAulaReposicao'), true);
                            //dijit.byId("cadAulaReposicao").show();
                            //IncluirAlterar(0, 'divAlterarAulaReposicao', 'divIncluirAulaReposicao', 'divExcluirAulaReposicao', 'apresentadorMensagemAulaReposicao', 'divCancelarAulaReposicao', 'divLimparAulaReposicao');

                        } else
                            apresentaMensagem('apresentadorMensagem', dataAulaReposicao);

                    }
                    catch (e) {
                        hideCarregando();
                        postGerarLog(e);
                    }
                },
                function (error) {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemAulaReposicao', error.response.data);
                });
            });
        }
    }
    catch (er) {
        hideCarregando();
        postGerarLog(er);
    }
}

//function obterRecursosAulaReposicaoDbClick(aulaReposicaoPesq, cursos, gridAulaReposicao) {
//    //showCarregando();

//    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
//        xhr.post(Endereco() + "/coordenacao/obterRecursosAulaReposicao", {
//            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
//            handleAs: "json",
//            data: ref.toJson(aulaReposicaoPesq)
//        }).then(function (dataAulaReposicao) {
//            try {
//                showCarregando();
//                if (!hasValue(dataAulaReposicao.erro)) {
//                    loadProfessor(dataAulaReposicao.retorno.professoresEdit, "cbResponsaveis");

//                    keepValues(null, gridAulaReposicao, false);
//                    changeActive(false);
//                    dojo.byId('cdUsuario').value = dataAulaReposicao.retorno.cd_atendente;
//                    dojo.byId('txAtendente').value = dataAulaReposicao.retorno.no_usuario;
//                    changeActive(true);
//                    dijit.byId("cadAulaReposicao").show();
//                    dijit.byId("tagAlunos").set("open", false);

//                } else {
//                    apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
//                }
//            } catch (e) {
//                hideCarregando();
//                postGerarLog(e);
//            }
//            //showCarregando();
//        })
//    })
//}

function obterRecursosAulaReposicaoDbClick(dataAulaReposicao, gridAulaReposicao) {
    try {
        showCarregando();
        if (!hasValue(dataAulaReposicao.erro)) {
            loadProfessor(dataAulaReposicao.retorno.professoresEdit, "cbResponsaveis");
            loadSala(dataAulaReposicao.retorno.salasDisponiveis, "cbSalaCad");
            keepValues(null, gridAulaReposicao, false);
            changeActive(false);
            dojo.byId('cdUsuario').value = dataAulaReposicao.retorno.cd_atendente;
            dojo.byId('txAtendente').value = dataAulaReposicao.retorno.no_usuario;
            changeActive(true);
            dijit.byId("cadAulaReposicao").show();
            dijit.byId("tagAlunos").set("open", false);

        } else {
            apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
        }
    } catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

//function obterRecursosAulaReposicaoClickCancel(aulaReposicaoPesq, cursos, gridAulaReposicaoCancel) {
//    //showCarregando();
//    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
//        xhr.post(Endereco() + "/coordenacao/obterRecursosAulaReposicao", {
//            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
//            handleAs: "json",
//            data: ref.toJson(aulaReposicaoPesq)
//        }).then(function (dataAulaReposicao) {
//            try {
//                if (!hasValue(dataAulaReposicao.erro)) {
//                    loadProfessor(dataAulaReposicao.retorno.professoresEdit, "cbResponsaveis");


                    
//                    keepValues(null, gridAulaReposicaoCancel, false);

//                    changeActive(false);
//                    dojo.byId('cdUsuario').value = dataAulaReposicao.retorno.cd_atendente;
//                    dojo.byId('txAtendente').value = dataAulaReposicao.retorno.no_usuario;
//                    changeActive(true);

//                } else {
//                    apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
//                }
//            } catch (e) {
//                hideCarregando();
//                postGerarLog(e);
//            }
//        })
//    })
//}

function obterRecursosAulaReposicaoClickCancel(dataAulaReposicao, gridAulaReposicaoCancel) {
    try {
        if (!hasValue(dataAulaReposicao.erro)) {
            loadProfessor(dataAulaReposicao.retorno.professoresEdit, "cbResponsaveis");
            loadSala(dataAulaReposicao.retorno.salasDisponiveis, "cbSalaCad");
            keepValues(null, gridAulaReposicaoCancel, false);

            changeActive(false);
            dojo.byId('cdUsuario').value = dataAulaReposicao.retorno.cd_atendente;
            dojo.byId('txAtendente').value = dataAulaReposicao.retorno.no_usuario;
            changeActive(true);

        } else {
            apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
        }
    } catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function obterRecursosAulaReposicao(aulaReposicaoPesq, itensSelecionados) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/coordenacao/obterRecursosAulaReposicao", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(aulaReposicaoPesq)
        }).then(function (dataAulaReposicao) {
            try {
                showCarregando();
                if (!hasValue(dataAulaReposicao.erro)) {
                    loadProfessor(dataAulaReposicao.retorno.professoresEdit, "cbResponsaveis");
                    loadSala(dataAulaReposicao.retorno.salasDisponiveis, "cbSalaCad");
                    dojo.byId('cdUsuario').value = dataAulaReposicao.retorno.cd_atendente;
                    itensSelecionados[0].no_usuario = dataAulaReposicao.retorno.no_usuario;
                    getLimpar('#formAulaReposicao');
                    apresentaMensagem('apresentadorMensagem', '');
                    dijit.byId('gridAulaReposicao').itemSelecionado = itensSelecionados[0];
                    keepValues(null, dijit.byId('gridAulaReposicao'), true);
                    dijit.byId("cadAulaReposicao").show();
                    IncluirAlterar(0, 'divAlterarAulaReposicao', 'divIncluirAulaReposicao', 'divExcluirAulaReposicao', 'apresentadorMensagemAulaReposicao', 'divCancelarAulaReposicao', 'divLimparAulaReposicao');
                } else {
                    apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
                }
            } catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        })
    })
}
//Edita um registro na grid de alunos atividade
function alterarAvaliacaoGrid(grid) {
    try {
        apresentaMensagem('apresentadorMensagemAluno', null);
        var cdAluno = dojo.byId("cd_aluno").value == "" ? 0 : dojo.byId("cd_aluno").value;
        for (var l = 0; l < grid._by_idx.length ; l++) {
            // Altera o registro na grade:
            if (grid._by_idx[l].item.cd_aluno == cdAluno) {
                grid._by_idx[l].item.ind_participacao = dojo.byId("ckParticipouAluno").checked;
                grid._by_idx[l].item.tx_observacao_aluno_aula = dojo.byId("descObs").value;
            }
        }
        grid.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//restaura dados da grade de aluno atividade
function keepValuesAluno(grid) {
    try {
        dojo.byId("cd_aluno").value = grid._by_idx[0].item.cd_aluno;
        dojo.byId("ckParticipouAluno").value = grid._by_idx[0].item.id_avaliacao_ativa == true ? dijit.byId("ckParticipouAluno").set("value", true) : dijit.byId("ckParticipouAluno").set("value", false);
        dojo.byId("descObs").value = grid._by_idx[0].item.tx_observacao_aluno_aula;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarAlunoAulaReposicao() {
    try {
        var alunos = [];
        var gridAlunoAulaReposicao = dijit.byId("gridAlunoAulaReposicao");

        hasValue(gridAlunoAulaReposicao) ? gridAlunoAulaReposicao.store.save() : null;
        if (hasValue(gridAlunoAulaReposicao) && hasValue(gridAlunoAulaReposicao._by_idx))
            var data = gridAlunoAulaReposicao._by_idx;
        else alunos = null;
        if (hasValue(gridAlunoAulaReposicao) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                alunos.push({
                    cd_aluno_aula_reposicao: val.item.cd_aluno_aula_reposicao,
                    cd_aula_reposicao: val.item.cd_aula_reposicao,
                    cd_aluno: val.item.cd_aluno,
                    id_participacao: val.item.ind_participacao == undefined && val.item.id_participacao != undefined ? val.item.id_participacao : val.item.ind_participacao,
                    tx_observacao_aluno_aula: val.item.tx_observacao_aluno_aula
                });
            });
        //gridAlunoAulaReposicao.listaCompletaAlunos = cloneArray(alunos);
        return alunos;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region Método para retornar registros na grade de Aluno - consistirNumeroVagas - ParticipouNao - pesquisarAlunoAulaReposicao
function retornarAlunoFK() {
    var nroVagas = hasValue(dijit.byId('nroVagas')) ? dijit.byId('nroVagas') : 0;
    /*if (nroVagas == 0) {
        caixaDialogo(DIALOGO_ERRO, msgNroVagasZerada, null);
        return false;
    }*/
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                var gridAlunoAulaReposicao = dijit.byId("gridAlunoAulaReposicao");
                var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

                if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    var storeGridAlunoAulaReposicao = (hasValue(gridAlunoAulaReposicao) && hasValue(gridAlunoAulaReposicao.store.objectStore.data)) ? gridAlunoAulaReposicao.store.objectStore.data : [];

                    if (storeGridAlunoAulaReposicao != null && storeGridAlunoAulaReposicao.length > 0) {
                        $.each(gridPesquisaAluno.itensSelecionados, function (idx, value) {
                            insertObjSort(gridAlunoAulaReposicao.store.objectStore.data, "cd_aluno", { cd_aluno: value.cd_aluno, no_pessoa: value.no_pessoa, tx_observacao_aluno_aula:"" });
                        });
                        gridAlunoAulaReposicao.setStore(new ObjectStore({ objectStore: new Memory({ data: gridAlunoAulaReposicao.store.objectStore.data }) }));

                    } else {
                        var dados = [];
                        //LBM Não tem vagas em aulas de reposição
                        //if (consistirNumeroVagas(gridPesquisaAluno.itensSelecionados) == false)
                        //    return false;
                        $.each(gridPesquisaAluno.itensSelecionados, function (index, val) {
                            insertObjSort(dados, "cd_aluno", { cd_aluno: val.cd_aluno, no_pessoa: val.no_pessoa, id_participacao: false, tx_observacao_aluno_aula: "" });
                        });
                        gridAlunoAulaReposicao.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));

                    }
                    gridAlunoAulaReposicao.listaCompletaAlunos = cloneArray(gridAlunoAulaReposicao.store.objectStore.data);
                    populaHorario();
                    dijit.byId("proAluno").hide();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function retornarAlunoPesquisaFK() {
    try {
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        /*if (hasValue(gridPesquisaAluno.itensSelecionados) && gridPesquisaAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
            return false;
        } else*/ {
            dojo.byId('cdAlunoPesq').value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId('alunoPesq').value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            //dojo.byId('desc_obs').value = "";
        }
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function consistirNumeroVagas(itens) {
    try {
        var gridAluno = hasValue(dijit.byId('gridAlunoAulaReposicao')) ? dijit.byId('gridAlunoAulaReposicao') : 0;
        var nroVagas = hasValue(dijit.byId('nroVagas')) ? dijit.byId('nroVagas').get('value') : 0;
        //if (nroVagas > 0) {
        var nroAlunos = 0;
        if (hasValue(itens) && itens.length > 0 && hasValue(gridAluno.store.objectStore.data))
            nroAlunos = itens.length + gridAluno.store.objectStore.data.length;
        if (nroAlunos > nroVagas) {
            caixaDialogo(DIALOGO_AVISO, msgNroVagasMenorAlunos, null);
            return false;
        } else return true;

        /* } else {
            caixaDialogo(DIALOGO_ERRO, msgNroVagasZerada, null);
            return false;
        }*/
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ParticipouNao(participou) {
    try {
        var itens = dijit.byId("gridAlunoAulaReposicao").itensSelecionados;
        if (!hasValue(itens) || itens.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAluno, null);
            return false;
        }
        else {
            var grid = dijit.byId("gridAlunoAulaReposicao").store.objectStore.data;
            for (var i = 0; i < grid.length; i++)
                for (var j = 0; j < itens.length; j++)
                    if (grid[i].cd_aluno == itens[j].cd_aluno)
                        if (participou) {
                            itens[j].id_participacao = true;
                            grid[i].ind_participacao = true;
                        }
                        else {
                            itens[j].id_participacao = false;
                            grid[i].id_participacao = false;
                        }
            dijit.byId("gridAlunoAulaReposicao").store.save();
            dijit.byId("gridAlunoAulaReposicao").update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

function retornarAlunoFKAtivdadeExtra() {
    if (!hasValue(dojo.byId("cadAulaReposicao").style.display))
        retornarAlunoFK();
    else
        retornarAlunoPesquisaFK()
}

function abrirAlunoFk() {
    dojo.byId('tipoRetornoAlunoFK').value = AULAREPOSICAO;
    dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    limparPesquisaAlunoFK();
    pesquisarAlunoFK(true,AULAREPOSICAO);
    dijit.byId("proAluno").show();
    dijit.byId('gridPesquisaAluno').update();
}

function abrirAlunoFkCad() {
    //var escola = jQuery.parseJSON(Escola());
    var cdTurma = dojo.byId("cdTurmaCad").value;
    dojo.byId('tipoRetornoAlunoFK').value = AULAREPOSICAOCADASTRO;
    dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    limparPesquisaAlunoFK();
    pesquisarAlunoFKAulaRep(true, AULAREPOSICAOCADASTRO,cdTurma);
    dijit.byId("proAluno").show();
    dijit.byId('gridPesquisaAluno').update();
}

function abrirTurmaFK() {
    try {
        dojo.byId("trAluno").style.display = "";
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = AULAREPOSICAO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK')))
            dijit.byId('tipoTurmaFK').set('value', 1);
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK(-1, AULAREPOSICAO);
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTurmaCadFK() {
    try {
        dojo.byId("trAluno").style.display = "";
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = AULAREPOSICAOCADASTRO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK')))
            dijit.byId('tipoTurmaFK').set('value', 1);
        if (hasValue(dijit.byId('cbSalaFK'))) {
            dijit.byId('cbSalaFK').set('value', 2);
            dijit.byId('cbSalaFK').set('disabled', true);

        }
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK(-1, AULAREPOSICAOCADASTRO);
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}


function retornarTurmaFKAulaReposicao() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == AULAREPOSICAO) {
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
                dojo.byId("cdTurmaPesq").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("turmaPesq").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limparTurmaPesc').set("disabled", false);
            }
            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKAulaReposicaoCadastro() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == AULAREPOSICAOCADASTRO) {
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
                var dataAtividade = hasValue(dojo.byId("dataAtividade").value) ? dojo.date.locale.parse(dojo.byId("dataAtividade").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                var dataTurma = hasValue(gridPesquisaTurmaFK.itensSelecionados[0].dtaIniAula) ? dojo.date.locale.parse(gridPesquisaTurmaFK.itensSelecionados[0].dtaIniAula, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                if (hasValue(dijit.byId('dataAtividade').value) && dataAtividade < dataTurma) {
                    caixaDialogo(DIALOGO_AVISO, msgHorarioAulaReposicao, null);
                    valido = false;
                }
                else {
                    dojo.byId("cdTurmaCad").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                    dojo.byId("cdProdutoTurmaCad").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_produto;
                    dojo.byId("cdCursoTurmaCad").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_curso;
                    dojo.byId("cdEstagioTurmaCad").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_estagio;

                    dojo.byId("dtIniTurmaCad").value = gridPesquisaTurmaFK.itensSelecionados[0].dtaIniAula;
                    dojo.byId("turmaCad").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                    dijit.byId('limparTurmaCad').set("disabled", false);
                }
            }
            var gridAlunoAulaReposicao = dijit.byId("gridAlunoAulaReposicao");
            gridAlunoAulaReposicao.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridAlunoAulaReposicao.update();
            populaHorario();

            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

// retorno Turma Destino 
function abrirTurmaFK() {
    try {
        dojo.byId("trAluno").style.display = "";
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = AULAREPOSICAO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK')))
            dijit.byId('tipoTurmaFK').set('value', 1);
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK(-1, AULAREPOSICAO);
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTurmaCadDestFK() {
    try {
        dojo.byId("trAluno").style.display = "";
        
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = AULAREPOSICAOCADASTROTURMADESTINO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK')))
            dijit.byId('tipoTurmaFK').set('value', 1);
        if (hasValue(dijit.byId('cbSalaFK'))) {
            dijit.byId('cbSalaFK').set('value', 2);
            dijit.byId('cbSalaFK').set('disabled', true);

        }


        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);

        if (hasValue(dojo.byId("cdCursoTurmaCad").value)) {
	        dijit.byId("pesCursoFK").set("value", parseInt(dojo.byId("cdCursoTurmaCad").value));
	        dijit.byId("pesCursoFK").set("disabled", true);
        }

        if (hasValue(dojo.byId("cdProdutoTurmaCad").value)) {
	        dijit.byId("pesProdutoFK").set("value", parseInt(dojo.byId("cdProdutoTurmaCad").value));
	        dijit.byId("pesProdutoFK").set("disabled", true);
        }

        dijit.byId("sPogramacaoFK").set("value", 1);
        dijit.byId("sPogramacaoFK").set("disabled", true);

        

        pesquisarTurmaAulaReposicaoDestinoFK(-1, AULAREPOSICAOCADASTROTURMADESTINO);
        

        

        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}


function retornarTurmaFKAulaReposicao() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == AULAREPOSICAO) {
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
                dojo.byId("cdTurmaPesq").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("turmaPesq").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limparTurmaPesc').set("disabled", false);
            }
            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaDestFKAulaReposicaoCadastro() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == AULAREPOSICAOCADASTROTURMADESTINO) {
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
                var dataAtividade = hasValue(dojo.byId("dataAtividade").value) ? dojo.date.locale.parse(dojo.byId("dataAtividade").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                var dataTurma = hasValue(gridPesquisaTurmaFK.itensSelecionados[0].dtaIniAula) ? dojo.date.locale.parse(gridPesquisaTurmaFK.itensSelecionados[0].dtaIniAula, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                if (hasValue(dijit.byId('dataAtividade').value) && dataAtividade < dataTurma) {
                    caixaDialogo(DIALOGO_AVISO, msgHorarioAulaReposicao, null);
                    valido = false;
                }
                else {

                    //preenche professor e desabilita
	                    var itemsProfessor = new Array({
		                    cd_pessoa: gridPesquisaTurmaFK.itensSelecionados[0].cd_professor,
                            no_pessoa: gridPesquisaTurmaFK.itensSelecionados[0].no_professor
	                    });
	                    
                        loadProfessor(itemsProfessor, "cbResponsaveis");
	                dijit.byId("cbResponsaveis")._onChangeActive = false;
                    dijit.byId("cbResponsaveis").set("value", gridPesquisaTurmaFK.itensSelecionados[0].cd_professor);
                    dijit.byId("cbResponsaveis")._onChangeActive = true;
                    dijit.byId("cbResponsaveis").set("disabled", true);

                    //preenche a sala e desabilita
                    if (gridPesquisaTurmaFK.itensSelecionados[0].cd_sala > 0 && hasValue(gridPesquisaTurmaFK.itensSelecionados[0].no_sala)) {
                        //cria o objeto para preencher o store da sala
	                    var itemsSala = new Array({
		                    cd_sala: gridPesquisaTurmaFK.itensSelecionados[0].cd_sala,
		                    no_sala: gridPesquisaTurmaFK.itensSelecionados[0].no_sala
                        });
                        //preenche o store
	                    loadSala(itemsSala, "cbSalaCad");

                        //seta a sala e desabilita o campo
                        dijit.byId("cbSalaCad")._onChangeActive = false;
                        dijit.byId("cbSalaCad").set("value", gridPesquisaTurmaFK.itensSelecionados[0].cd_sala);
                        dijit.byId("cbSalaCad")._onChangeActive = true;
                        dijit.byId("cbSalaCad").set("disabled", true);
	                    
                    }

                    //preenche os horarios e desabilita
                    if (hasValue(gridPesquisaTurmaFK.itensSelecionados[0].horarios)) {
                        var horariosTurma = gridPesquisaTurmaFK.itensSelecionados[0].horarios;
                        if (horariosTurma.length > 0) {
                            var dt_hora_ini_aux = horariosTurma.map((value) => {
		                        return value.dt_hora_ini;
	                        }).sort()[0];
	                        //Caso a turma tenha mais de 1 intervalo de horarios pega a maxima hora final
                            var dt_hora_fim_aux = horariosTurma.map((value) => {
		                        return value.dt_hora_fim;
                            }).sort().reverse()[0];

                            //preeche os horarios e desabilita os campos
                            loadHorarioGenerico(new Array(dt_hora_ini_aux), "cbHoraIni");
                            loadHorarioGenerico(new Array(dt_hora_fim_aux), "cbHoraFim");
                            
                            if (hasValue(dt_hora_ini_aux) && hasValue(dt_hora_fim_aux)) {
                                dijit.byId("cbHoraIni")._onChangeActive = false;
                                dijit.byId("cbHoraIni").set("value", dt_hora_ini_aux); 
                                dijit.byId("cbHoraIni")._onChangeActive = true;
                                dijit.byId("cbHoraIni").set("disabled", true);

                                dijit.byId("cbHoraFim")._onChangeActive = false;
                                dijit.byId("cbHoraFim").set("value", dt_hora_fim_aux);
                                dijit.byId("cbHoraFim")._onChangeActive = true;
                                dijit.byId("cbHoraFim").set("disabled", true);
                            }
                        }

                    }
                    

                    dojo.byId("cdTurmaDest").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                    dojo.byId("dtIniTurmaDest").value = gridPesquisaTurmaFK.itensSelecionados[0].dtaIniAula;
                    dojo.byId("turmaDest").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                    dijit.byId('limparTurmaDest').set("disabled", false);
                }
            }
            var gridAlunoAulaReposicao = dijit.byId("gridAlunoAulaReposicao");
            gridAlunoAulaReposicao.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridAlunoAulaReposicao.update();
            populaHorario();

            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
