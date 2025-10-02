define(["dojo/store/Memory", "dojo/store/Observable"],
    function (Memory, Observable) {
        

        function pesquisar() {
            console.log("pesquisarService");
        }

        function linkAcoesEditar() {

        }

        function linkAcoesExcluir() {

        }

        function novo() {

        }

        function visualizarRelatorio() {

        }

        function todosItens() {

        }


        function itensSelecionados() {

        }

        function editar() {

        }

        function limpar() {

        }

        function cancelar() {

        }

        function deletar() {

        }

        function rowDbClickGrid(evt) {
            console.log(evt);
        }

        function salvar() {

        }

        return {
            pesquisar: pesquisar,
            linkAcoesEditar: linkAcoesEditar,
            linkAcoesExcluir: linkAcoesExcluir,
            novo: novo,
            visualizarRelatorio: visualizarRelatorio,
            todosItens: todosItens,
            itensSelecionados: itensSelecionados,
            editar: editar,
            limpar: limpar,
            cancelar: cancelar,
            deletar: deletar,
            rowDbClickGrid: rowDbClickGrid,
            salvar: salvar
        };
    }
);