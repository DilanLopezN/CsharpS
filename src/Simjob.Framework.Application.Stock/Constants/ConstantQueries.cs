using Simjob.Framework.Domain.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Application.Stock.Constants
{
    public static class ConstantQueries
    {
        [ExcludeFromCodeCoverage]
        #region stockmovsum
        public static string GetStockMovSumAmountGroupByItem(string filter, string groupByProperty = "itemId")
        {
            string filterMatch = string.IsNullOrEmpty(filter) ? "" : $@"{{ $match: {filter} }},";
            string groupBy = $@"{{ $group: {{ '_id': '${groupByProperty}' , sum: {{ $sum: '$amount' }} }} }},";
            string query = @"{
                           aggregate: 'stockmov',
                          cursor: { },
                          pipeline: [
                         
                            
                          {
                              $lookup: {
                                  from: 'stockoperation',
                                  localField: 'stockOperationId',
                                  foreignField: '_id',
                                  as: 'stockoperation'
                              }
                          },
                             {
                              $unwind: {
                                  'path': '$stockoperation',
                                  'preserveNullAndEmptyArrays': true,
                            }
                          },
                          {
                              $lookup: {
                                  from: 'stocklocal',
                                  localField: 'stockLocalId',
                                  foreignField: '_id',
                                  as: 'stocklocal'
                              }
                          },
                            {
                              $unwind: {
                                  'path': '$stocklocal',
                                  'preserveNullAndEmptyArrays': true,
                            }
                          },
                             {
                              $lookup: {
                                  from: 'stockoperationtype',
                                  localField: 'stockoperation.stockOperationTypeId',
                                  foreignField: '_id',
                                  as: 'stockoperationtype'
                              }
                          },
                              
                            {
                              $unwind: {
                                  'path': '$stockoperationtype',
                                  'preserveNullAndEmptyArrays': true,
                            }
                        
                          },
                           
                           <filter>
                           <groupBy>
                         ]
                        }";

            return query.Replace("<filter>", filterMatch)
                        .Replace("<groupBy>", groupBy);
        }
        #endregion

        #region nf
        public static string GetAllNfByStockOperation(string stockOperationId = "")
        {
            string query = @"{
    aggregate: 'fisdocrec',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },
                    { stockOperation: '<FilterStockOperationId>' },
                ]
            }
        },
        {
            $lookup: {
                from: 'fisdocrecitem',
                localField: 'fisDocRecItemS',
                foreignField: '_id',
                as: 'fisdocrecitem',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { qtd_recebido: { $exists: true } },
                                { qty: { $exists: true, $ne: null } },
                                { $expr: { $lt: ['$qtd_recebido', '$qty'] } }
                            ]
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                qty: '$qty',
                                qtd_recebido: '$qtd_recebido',
                            }
                        }
                    }
                ]
            }
        },
   
        {
            $set: {
                documentCount: { $size: '$fisdocrecitem' }
            }
        },
        {
            $group: {
                _id: null,
                totalDocumentCount: { $sum: '$documentCount' }
            }
        },
        {
            $project: {
                _id: 0,
                totalDocumentCount: 1
            }
        }
    ]
}";

            query = query.Replace("<FilterStockOperationId>", stockOperationId);

            return query;

        }
        #endregion

        #region local
        public static string GetAllLocal(LocalFilterModel model)
        {
            string filterCodigoLocal = "";
            if (!string.IsNullOrEmpty(model.CodigoLocal))
            {
                filterCodigoLocal = @"{ code: '<FilterCodigoLocal>' },";
                filterCodigoLocal = filterCodigoLocal.Replace("<FilterCodigoLocal>", model.CodigoLocal);
            }
            else
            {
                filterCodigoLocal = @"{ code: { $regex: '', $options: 'i' } },";
            }


            string query = @"{
    aggregate: 'stocklocal',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },
                    <FilterCodigoLocal>

                ]
            }
        },
        {
            $lookup: {
                from: 'stockvol',
                localField: '_id',
                foreignField: 'stockLocalId',
                as: 'volume',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { isDeleted: false },

                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: 'stockvolcont',
                            localField: 'stockVolCont',
                            foreignField: '_id',
                            as: 'stockvolcont',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { isDeleted: false },
                                            { quantidade: { $gt: 0 } },

                                        ]
                                    }
                                },
                                {
                                    $lookup: {
                                        from: 'item',
                                        localField: 'idItem',
                                        foreignField: '_id',
                                        as: 'item',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { isDeleted: false },

                                                    ]
                                                }
                                            },
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id: '$_id',
                                                        codigo: '$codeExt',
                                                        descricao: '$description',
                                                        unidadeId: '$unitId',
                                                        unidadeNome: '$unitName',
                                                        loteExpirateDate: '$loteExpirateDate'
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$item',
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            loteCode: '$loteCode',
                                            quantidade: '$quantidade',
                                            item: '$item'
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$stockvolcont',
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                codigo: '$code',
                                dateVol: '$dateVol',
                                origemCode: '$origemCode',
                                stockLocalCode: '$stockLocalCode',
                                stockvolcont: '$stockvolcont',
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$volume',
            }
        },


        {
            $sort: { 'volume.codigo': -1 }
        },
        {
            $replaceRoot: {
                newRoot: {
                    Id: { $ifNull: ['$volume.id', ''] },
                    CodigoVolume: { $ifNull: ['$volume.codigo', ''] },
                    IdItem: { $ifNull: ['$volume.stockvolcont.item.id', ''] },
                    CodigoItem: { $ifNull: ['$volume.stockvolcont.item.codigo', ''] },
                    DescricaoItem: { $ifNull: ['$volume.stockvolcont.item.descricao', ''] },
                    UnidadeId: { $ifNull: ['$volume.stockvolcont.item.unidadeId', ''] },
                    UnidadeNome: { $ifNull: ['$volume.stockvolcont.item.unidadeNome', ''] },
                    CodigoLocal: { $ifNull: ['$volume.stockLocalCode', ''] },
                    CodigoLote: { $ifNull: ['$volume.stockvolcont.loteCode', ''] },
                    ValidadeLote: { $ifNull: ['$loteExpirateDate', ''] },
                    Quantidade: { $ifNull: ['$volume.stockvolcont.quantidade', ''] },
                    DataVolume: { $ifNull: ['$volume.dateVol', ''] },
                    Origem: { $ifNull: ['$volume.origemCode', ''] }
                }
            }
        },
    ]
}";
            query = query.Replace("<FilterCodigoLocal>", filterCodigoLocal);
            return query;
        }
        #endregion

        #region volume
        public static string GetAllVolume(VolumeFilterModel model)
        {
            string filterVolCode = "";
            if (!string.IsNullOrEmpty(model.CodigoVolume))
            {
                filterVolCode = @"{ code: '<FilterCodigoVolume>' },";
                filterVolCode = filterVolCode.Replace("<FilterCodigoVolume>", model.CodigoVolume);
            }
            else
            {
                filterVolCode = @"{ code: { $regex: '', $options: 'i' } },";
            }
            string query = @"{
    aggregate: 'stockvol',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },                
                    <FilterCodigoVolume>

                ]
            }
        },
        {
            $lookup: {
                from: 'stockvolcont',
                localField: 'stockVolCont',
                foreignField: '_id',
                as: 'stockvolcont',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { isDeleted: false },
                                { quantidade: { $gt: 0 } },

                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: 'item',
                            localField: 'idItem',
                            foreignField: '_id',
                            as: 'item',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { isDeleted: false },

                                        ]
                                    }
                                },

                                {
                                    $lookup: {
                                        from: 'stockbalance',
                                        localField: '_id',
                                        foreignField: 'itemId',
                                        as: 'stockbalance',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { isDeleted: false },
                                                        { qtTotal: { $gt: 0 } }

                                                    ]
                                                }
                                            },
                                            {
                                                $sort: {
                                                    date: -1
                                                }
                                            },
                                            {
                                                $limit: 1
                                            },

                                            {
                                                $lookup: {
                                                    from: 'stocklocal',
                                                    localField: 'stockLocId',
                                                    foreignField: '_id',
                                                    as: 'local',
                                                    pipeline: [
                                                        {
                                                            $match: {
                                                                $and: [
                                                                    { isDeleted: false },
                                                                ]
                                                            }
                                                        },

                                                        {
                                                            $replaceRoot: {
                                                                newRoot: {
                                                                    id: '$_id',
                                                                    codigo: '$code',
                                                                }
                                                            }
                                                        }
                                                    ]
                                                }
                                            },
                                            {
                                                $unwind: {
                                                    path: '$local',
                                                }
                                            },

                                            {
                                                $lookup: {
                                                    from: 'stocklot',
                                                    localField: 'stockLotId',
                                                    foreignField: '_id',
                                                    as: 'lote',
                                                    pipeline: [
                                                        {
                                                            $match: {
                                                                $and: [
                                                                    { isDeleted: false },

                                                                ]
                                                            }
                                                        },

                                                        {
                                                            $replaceRoot: {
                                                                newRoot: {
                                                                    id: '$_id',
                                                                    codigo: '$code',
                                                                    validade: '$expirateDate'
                                                                }
                                                            }
                                                        }
                                                    ]
                                                }
                                            },
                                            {
                                                $unwind: {
                                                    path: '$lote',
                                                }
                                            },


                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id: '$_id',
                                                        lote: '$lote',
                                                        local: '$local',
                                                        qtdVolumes: '$qtTotal'
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$stockbalance',                                    
                                        
                                    }
                                },

                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$codeExt',
                                            descricao: '$description',
                                            unidadeId: '$unitId',
                                            unidadeNome: '$unitName',
                                            stockbalance: '$stockbalance'
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$item',
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                codigo: '$code',
                                item: '$item',
                                quantidade: '$quantidade',
                                quantidadePicking: '$quantidade_picking'
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$stockvolcont',
            }
        },
        {
            $sort: { 'code': -1 }
        },
        {
            $replaceRoot: {
                newRoot: {
                    Id: { $ifNull: ['$_id', ''] },
                    CodigoVolume: { $ifNull: ['$code', ''] },
                    CodigoItem: { $ifNull: ['$stockvolcont.item.codigo', ''] },
                    DescricaoItem: { $ifNull: ['$stockvolcont.item.descricao', ''] },
                    UnidadeId: { $ifNull: ['$stockvolcont.item.unidadeId', ''] },
                    UnidadeNome: { $ifNull: ['$stockvolcont.item.unidadeNome', ''] },
                    CodigoLocal: { $ifNull: ['$stockvolcont.item.stockbalance.local.codigo', ''] },
                    CodigoLote: { $ifNull: ['$stockvolcont.item.stockbalance.lote.codigo', ''] },
                    ValidadeLote: { $ifNull: ['$stockvolcont.item.stockbalance.lote.validade', ''] },
                    Quantidade: { $ifNull: ['$stockvolcont.quantidade', ''] },
                    QuantidadePicking: { $ifNull: ['$stockvolcont.quantidadePicking', ''] },
                    DataVolume: { $ifNull: ['$dateVol', ''] },
                    Origem: { $ifNull: ['$origemCode', ''] }
                }
            }
        },
    ]
}";
            query = query.Replace("<FilterCodigoVolume>", filterVolCode);
            return query;

        }

        #endregion



        #region lote
        public static string GetAllLote(LoteFilterModel model)
        {
            string filterDateInicioValidade = model.DataInicioValidade.HasValue ? model.DataInicioValidade.ToString() : DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss");
            string filterDateFinalValidade = model.DataFimValidade.HasValue ? model.DataFimValidade.ToString() : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss");

            string filterDataExpiracao = "";

            if (model.DataFimSaldo.HasValue && model.DataFimSaldo.HasValue)
            {
                filterDataExpiracao = @"{
                        $match: {
                            $and: [
                                { 'expirateDate': {$gte:ISODate('<FilterDtInicio>'),$lte:ISODate('<FilterDtFinal>')}}
                            ]
                        }
                    },";
                filterDataExpiracao = filterDataExpiracao.Replace("<FilterDtInicio>", filterDateInicioValidade);
                filterDataExpiracao = filterDataExpiracao.Replace("<FilterDtFinal>", filterDateFinalValidade);
            }



            string filterDateInicio = model.DataInicioSaldo.HasValue ? model.DataInicioSaldo.ToString() : DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss");
            string filterDateFinal = model.DataFimSaldo.HasValue ? model.DataFimSaldo.ToString() : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss");
            string limit = "";
            string filterDataSaldo = "";

            if (model.DataFimSaldo.HasValue && model.DataFimSaldo.HasValue)
            {
                filterDataSaldo = @"{
                        $match: {
                            $and: [
                                { 'date': {$gte:ISODate('<FilterDtInicioSaldo>'),$lte:ISODate('<FilterDtFinalSaldo>')}}
                            ]
                        }
                    },";
                filterDataSaldo = filterDataExpiracao.Replace("<FilterDtInicioSaldo>", filterDateInicio);
                filterDataSaldo = filterDataExpiracao.Replace("<FilterDtFinalSaldo>", filterDateFinal);
            }
            else
            {
                limit = @"{
                $limit: 1
            },";
            }


            string filterLoteCode = "";
            if (!string.IsNullOrEmpty(model.CodLote))
            {
                filterLoteCode = @"{ code: '<FilterLoteCode>' },";
                filterLoteCode = filterLoteCode.Replace("<FilterLoteCode>", model.CodLote);
            }
            else
            {
                filterLoteCode = @"{ code: { $regex: '', $options: 'i' } },";
            }



            string query = @"{
    aggregate: 'stocklot',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },
                    <FilterLoteCode>
                ]
            }
        },
        {
            $lookup: {
                from: 'stockvolcont',
                localField: '_id',
                foreignField: 'idLote',
                as: 'stockvolcont',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { isDeleted: false },
                                { quantidade: { $gt: 0 } },
                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: 'stockvol',
                            localField: '_id',
                            foreignField: 'stockVolCont',
                            as: 'volume',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { isDeleted: false },
                                        ]
                                    }
                                },

                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$code',
                                            dateVol: '$dateVol',
                                            origemCode: '$origemCode',
                                            stockLocalCode: '$stockLocalCode',
                                            stockLocalId: '$stockLocalId',
                                        }
                                    }
                                },
                                {
                                    $lookup: {
                                        from: 'stocklocal',
                                        localField: 'stockLocalId',
                                        foreignField: '_id',
                                        as: 'stocklocal',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { isDeleted: false },
                                                        { isAvailable: true },
                                                    ]
                                                }
                                            },
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$stocklocal',
                                    }
                                },
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$volume',
                        }
                    },
                    {
                        $lookup: {
                            from: 'item',
                            localField: 'idItem',
                            foreignField: '_id',
                            as: 'item',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { isDeleted: false },

                                        ]
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$codeExt',
                                            descricao: '$description',
                                            unidadeId: '$unitId',
                                            unidadeNome: '$unitName',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$item',
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                loteCode: '$loteCode',
                                volume: '$volume',
                                item: '$item',
                                quantidade: '$quantidade'
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$stockvolcont',
            }
        },
        {
            $sort: { 'stockvolcont.volume.codigo': -1 }
        },
        {
            $replaceRoot: {
                newRoot: {
                    Id: { $ifNull: ['$stockvolcont.volume.id', ''] },
                    CodigoVolume: { $ifNull: ['$stockvolcont.volume.codigo', ''] },
                    IdItem: { $ifNull: ['$stockvolcont.item.id', ''] },
                    CodigoItem: { $ifNull: ['$stockvolcont.item.codigo', ''] },
                    DescricaoItem: { $ifNull: ['$stockvolcont.item.descricao', ''] },
                    UnidadeId: { $ifNull: ['$stockvolcont.item.unidadeId', ''] },
                    UnidadeNome: { $ifNull: ['$stockvolcont.item.unidadeNome', ''] },
                    CodigoLocal: { $ifNull: ['$stockvolcont.volume.stockLocalCode', ''] },
                    CodigoLote: { $ifNull: ['$code', ''] },
                    ValidadeLote: { $ifNull: ['$expirateDate', ''] },
                    Quantidade: { $ifNull: ['$stockvolcont.quantidade', ''] },
                    DataVolume: { $ifNull: ['$volume.dateVol', ''] },
                    Origem: { $ifNull: ['$volume.origemCode', ''] }
                }
            }
        },
    ]
}";
            //query = query.Replace("<FilterLoteId>", model.IdLote);
            query = query.Replace("<FilterLoteCode>", filterLoteCode);
            //query = query.Replace("<FilterLocalId>", model.IdLocal);
            //query = query.Replace("<FilterLocalCode>", model.CodLocal);
            //query = query.Replace("<FilterExpirateDate>", filterDataExpiracao);
            //query = query.Replace("<FilterDateSaldo>", filterDataSaldo);

            query = query.Replace("<Limit>", limit);

            return query;

        }
        #endregion

        #region item
        public static string GetAllItem(ItemFilterModel model)
        {
            string filterDateInicio = model.DataInicioSaldo.HasValue ? model.DataInicioSaldo.ToString() : DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss");
            string filterDateFinal = model.DataFimSaldo.HasValue ? model.DataFimSaldo.ToString() : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss");
    
            
            string filterCodigoItem = "";
            if (!string.IsNullOrEmpty(model.CodigoItem))
            {
                filterCodigoItem = @"{ codeExt: '<FilterCodigoItem>' },";
                filterCodigoItem = filterCodigoItem.Replace("<FilterCodigoItem>", model.CodigoItem);
            }
            else
            {
                filterCodigoItem = @"{ codeExt: { $regex: '', $options: 'i' } },";
            }
            string query = @"{
    aggregate: 'item',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },
                    <FilterCodigoItem>

                ]
            }
        },
        {
            $lookup: {
                from: 'stockvolcont',
                localField: '_id',
                foreignField: 'idItem',
                as: 'stockvolcont',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { isDeleted: false },
                                { quantidade: { $gt: 0 } },

                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: 'stockvol',
                            localField: '_id',
                            foreignField: 'stockVolCont',
                            as: 'volume',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { isDeleted: false },


                                        ]
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$code',
                                            dateVol: '$dateVol',
                                            origemCode: '$origemCode',
                                            stockLocalCode: '$stockLocalCode',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$volume',
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                loteCode: '$loteCode',
                                volume: '$volume',
                                quantidade: '$quantidade'
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$stockvolcont',
            }
        },


        {
            $sort: { 'volume.codigo': -1 }
        },
        {
            $replaceRoot: {
                newRoot: {
                    Id: { $ifNull: ['$stockvolcont.volume.id', ''] },
                    CodigoVolume: { $ifNull: ['$stockvolcont.volume.codigo', ''] },
                    IdItem: { $ifNull: ['$_id', ''] },
                    CodigoItem: { $ifNull: ['$codeExt', ''] },
                    DescricaoItem: { $ifNull: ['$description', ''] },
                    UnidadeId: { $ifNull: ['$unitId', ''] },
                    UnidadeNome: { $ifNull: ['$unitName', ''] },
                    CodigoLocal: { $ifNull: ['$stockvolcont.volume.stockLocalCode', ''] },
                    CodigoLote: { $ifNull: ['$stockvolcont.loteCode', ''] },
                    ValidadeLote: { $ifNull: ['$loteExpirateDate', ''] },
                    Quantidade: { $ifNull: ['$stockvolcont.quantidade', ''] },
                    DataVolume: { $ifNull: ['$volume.dateVol', ''] },
                    Origem: { $ifNull: ['$volume.origemCode', ''] }
                }
            }
        },
    ]
}";

            query = query.Replace("<FilterCodigoItem>", filterCodigoItem);
            return query;
        }
        #endregion

        #region itemRule
        public static string GetAllItemRule(ItemRuleFilterModel model)
        {
            string filterLocal = "";
            string filterFamilia = "";
            if (!string.IsNullOrEmpty(model.Local))
            {
                filterLocal = @"{ stockLocalCode: '<FilterLocal>' },";
                filterLocal = filterLocal.Replace("<FilterLocal>", model.Local);
            }
            else
            {
                filterLocal = @"{ stockLocalCode: { $regex: '', $options: 'i' } },";
            }


            if (!string.IsNullOrEmpty(model.Familia))
            {
                filterFamilia = @"{ seg: '<FilterFamilia>' },";
                filterFamilia = filterFamilia.Replace("<FilterFamilia>", model.Familia);
            }
            else
            {
                filterFamilia = @"{ seg: { $regex: '', $options: 'i' } },";
            }
            string query = @"{
    aggregate: 'item',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },
                    <FilterFamilia>
                ]
            }
        },
        {
            $lookup: {
                from: 'unit',
                localField: 'unitId',
                foreignField: '_id',
                as: 'unidade',
                pipeline: [
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                descricao: '$description',
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$unidade',
                preserveNullAndEmptyArrays: true
            }
        },

        {
            $lookup: {
                from: 'rulestocklocal',
                localField: 'ruleStockLocal',
                foreignField: '_id',
                as: 'rulestocklocal',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                 <FilterLocal>
                            ]
                        }
                    },
                    {
                    $lookup: {
                        from: 'stocklocal',
                        localField: 'stockLocal',
                        foreignField: '_id',
                        as: 'local',
                        pipeline: [
                            {
                                $replaceRoot: {
                                    newRoot: {
                                        id: '$_id',
                                        descricao: '$description',
                                    }
                                }
                            }
                         ]
                       }
                    },
                    {
                        $unwind: {
                            path: '$local',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                stockLocalId: '$stockLocal',
                                stockLocalCode: '$stockLocalCode',
                                stockLocalDescription: '$local.descricao',
                                qtdMin: '$qtdMin',
                                qtdMax: '$qtdMax',
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$rulestocklocal',
                preserveNullAndEmptyArrays: true
            }
        },

        {
            $sort: { codeExt: -1 }
        },
        {
            $replaceRoot: {
                newRoot: {
                    Id: { $ifNull: ['$_id', ''] },
                    CodProduto: { $ifNull: ['$codeExt', ''] },
                    Descricao: { $ifNull: ['$description', ''] },
                    Unidade: { $ifNull: ['$unidade.descricao', ''] },
                    LocalId: { $ifNull: ['$rulestocklocal.stockLocalId', ''] },
                    Local: { $ifNull: ['$rulestocklocal.stockLocalCode', ''] },
                    DescricaoLocal: { $ifNull: ['$rulestocklocal.stockLocalDescription', ''] },
                    QtdMin: { $ifNull: ['$rulestocklocal.qtdMin', ''] },
                    QtdMax: { $ifNull: ['$rulestocklocal.qtdMax', ''] },
                }
        }
        },
    ]
}";
                query = query.Replace("<FilterLocal>", filterLocal);
                query = query.Replace("<FilterFamilia>", filterFamilia);
                return query;

        }
        #endregion

        #region itemRuleRepoReab
        public static string GetAllItemRuleReopReab(ItemRuleFilterModel model)
        {
            string filterLocal = "";
            if (!string.IsNullOrEmpty(model.Local))
            {
                filterLocal = @"{ stockLocalCode: '<FilterLocal>' },";
                filterLocal = filterLocal.Replace("<FilterLocal>", model.Local);
            }


            string filterFamilia = "";
            if (!string.IsNullOrEmpty(model.Familia))
            {
                filterFamilia = @"{ seg: '<FilterFamilia>' },";
                filterFamilia = filterFamilia.Replace("<FilterFamilia>", model.Familia);
            }

            string filterItemId = "";
            if (!string.IsNullOrEmpty(model.ItemId))
            {
                filterItemId = @"{ _id: '<FilterItemId>' },";
                filterItemId = filterItemId.Replace("<FilterItemId>", model.ItemId);
            }

            string filterItemDescription = "";
            if (!string.IsNullOrEmpty(model.ItemDescription))
            {
                filterItemDescription = @"{ _id: '<FilterItemDescription>' },";
                filterItemDescription = filterItemDescription.Replace("<FilterItemDescription>", model.ItemDescription);
            }

            string query = @"{
    aggregate: 'item',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },
                    <FilterFamilia>
                    <FilterItemId>
                    <FilterItemDescription>
                ]
            }
        },
        {
            $lookup: {
                from: 'unit',
                localField: 'unitId',
                foreignField: '_id',
                as: 'unidade',
                pipeline: [
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                descricao: '$description',
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$unidade',
                preserveNullAndEmptyArrays: true
            }
        },
        {
            $lookup: {
                from: 'rulestocklocal',
                localField: 'ruleStockLocal',
                foreignField: '_id',
                as: 'rulestocklocal',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { isDeleted: false },
                                <FilterLocal>
                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: 'stocklocal',
                            localField: 'stockLocal',
                            foreignField: '_id',
                            as: 'local',
                            pipeline: [
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            descricao: '$description',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$local',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                stockLocalId: '$stockLocal',
                                stockLocalCode: '$stockLocalCode',
                                stockLocalDescription: '$local.descricao',
                                qtdMin: '$qtdMin',
                                qtdMax: '$qtdMax',
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$rulestocklocal',
                preserveNullAndEmptyArrays: true
            }
        },
        {
            $sort: { codeExt: -1 }
        },
        {
            $replaceRoot: {
                newRoot: {
                    Id: { $ifNull: ['$_id', ''] },
                    Seg: { $ifNull: ['$seg', ''] },
                    CodProduto: { $ifNull: ['$codeExt', ''] },
                    Descricao: { $ifNull: ['$description', ''] },
                    Unidade: { $ifNull: ['$unidade.descricao', ''] },
                    LocalId: { $ifNull: ['$rulestocklocal.stockLocalId', ''] },
                    Local: { $ifNull: ['$rulestocklocal.stockLocalCode', ''] },
                    DescricaoLocal: { $ifNull: ['$rulestocklocal.stockLocalDescription', ''] },
                    QtdMin: { $ifNull: ['$rulestocklocal.qtdMin', ''] },
                    QtdMax: { $ifNull: ['$rulestocklocal.qtdMax', ''] },
                }
            }
        },
        {
            $match: {
                QtdMax: { $ne: '' }
            }
        }
    ]
}";
            query = query.Replace("<FilterLocal>", filterLocal);
            query = query.Replace("<FilterFamilia>", filterFamilia);
            query = query.Replace("<FilterItemId>", filterItemId);
            query = query.Replace("<FilterItemDescription>", filterItemDescription);
            return query;

        }
        #endregion

        #region quantidade pendente de picking
        public static string GetQtdPendingPicking(string? familia)
        {
            string filterFamilia = "";

            if (!string.IsNullOrEmpty(familia))
            {
                filterFamilia = @"{ familia: '<FilterFamilia>' },";
                filterFamilia = filterFamilia.Replace("<FilterFamilia>", familia);
            }
            else
            {
                filterFamilia = @"{ familia: { $regex: '', $options: 'i' } },";
            }
            string query = @"{
                ""aggregate"": ""picking"",
                ""cursor"": { 'batchSize': 100000000 },
                ""pipeline"": [
                    {
                        $match: {
                            $and: [
                                { ""isDeleted"": false }, 
                                { status: { $nin: [""Cancelado"", ""Finalizado""] } },
                                <FilterFamilia>
                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: ""pickingitem"",
                            localField: ""itemsPicking"",
                            foreignField: ""_id"",
                            as: ""pickingitem"",
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { $expr: { $gt: [""qtdSeparate"", ""qtd""] } }
                                        ]
                                    }
                                },
                            ]
                        }
                    },
                    {
                        $unwind: {
                            ""path"": ""$pickingitem"",
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                item_id: { $ifNull: ['$pickingitem.itemId', ''] },
                                qtdpendente: { $subtract: [""$pickingitem.qtd"", ""$pickingitem.qtdSeparate""] },
                                familia: { $ifNull: ['$familia', ''] },
                            }
                        }
                    },
                    {
                        $group: {
                            _id: ""$item_id"",
                            itemId: { $first: ""$item_id"" },
                            totalQtdPendente: { $sum: ""$qtdpendente"" },
                            seg: { $first: ""$familia"" } 
                        }
                    }
                ]
            }";
            query = query.Replace("<FilterFamilia>", filterFamilia);
            return query;
        }

        #endregion


        #region operMov
        public static string GetOperMov(OperMovFilterModel model)
        {
            string matchDate = "";
            if (model.DataInicioReal != null && model.DataInicioReal != null)
            {
                matchDate = @"{ dateReal: {$gte:ISODate('<FilterDtInicio>'),$lte:ISODate('<FilterDtFinal>')}}";
                matchDate = matchDate.Replace("<FilterDtInicio>", model.DataInicioReal.ToString());
                matchDate = matchDate.Replace("<FilterDtFinal>", model.DataFimReal.ToString());
            }
            //  { dateReal: {$gte:ISODate(2024-01-18T00:00:00),$lte:ISODate(2024-01-18T23:59:59)}}

            string query = @"{
    aggregate: 'stockoperation',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },
                    { _id: { $regex: '<FilterIdOperacao>', $options: 'i' } },
                    { code: { $regex: '<FilterCodigoOperacao>', $options: 'i' } },  
                    <FilterDate>
                ]
            }
        },
        {
            $lookup: {
                from: 'stockmov',
                localField: 'stockMovIds',
                foreignField: '_id',
                as: 'stockmov',
                pipeline: [
                    {
                        $match: {
                            $and: [
                               { qty: { $gt: 0 } }
                                
                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: 'stockvol',
                            localField: 'stockVolId',
                            foreignField: '_id',
                            as: 'stockvol',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { _id: { $regex: '<FilterIdVol>', $options: 'i' } },
                                            { code: { $regex: '<FilterCodigoVol>', $options: 'i' } },
                                        ]
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$code',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$stockvol',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $lookup: {
                            from: 'stocklot',
                            localField: 'stockLotId',
                            foreignField: '_id',
                            as: 'stocklot',
                            pipeline: [
                                 {
                                    $match: {
                                        $and: [
                                            { _id: { $regex: '<FilterLoteId>', $options: 'i' } },
                                            { code: { $regex: '<FilterLoteCodigo>', $options: 'i' } },
                                        ]
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$code',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$stocklot',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $lookup: {
                            from: 'stocklocal',
                            localField: 'stockLocalFromId',
                            foreignField: '_id',
                            as: 'stocklocal',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { _id: { $regex: '<FilterLocalId>', $options: 'i' } },
                                            { code: { $regex: '<FilterLocalCodigo>', $options: 'i' } },
                                            { description: { $regex: '', $options: 'i' } },
                                        ]
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$code',
                                            descricao: '$description',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$stocklocal',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $lookup: {
                            from: 'item',
                            localField: 'itemId',
                            foreignField: '_id',
                            as: 'item',
                            pipeline: [
                                 {
                                    $match: {
                                        $and: [
                                            { _id: { $regex: '<FilterItemId>', $options: 'i' } },
                                            { codeExt: { $regex: '<FilterItemCodigo>', $options: 'i' } },
                                        ]
                                    }
                                },
                                {
                                    $lookup: {
                                        from: 'unit',
                                        localField: 'unitId',
                                        foreignField: '_id',
                                        as: 'unidade',
                                        pipeline: [
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id: '$_id',
                                                        descricao: '$description',
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$unidade',
                                        preserveNullAndEmptyArrays: true
                                    }
                                },

                                {
                                    $lookup: {
                                        from: 'stockbalance',
                                        localField: '_id',
                                        foreignField: 'itemId',
                                        as: 'stockbalance',
                                        pipeline: [
                                             {
                                                 $match: {
                                                       $and: [
                                                               { qtTotal: { $gt: 0 } }
                                
                                                             ]
                                                        }
                                                    },
                                             
                                             {
                                                $sort: {
                                                    date: -1
                                                }
                                              },
                                              {
                                                 $limit: 1
                                              },

                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id: '$_id',
                                                        qtdVolumes: '$qtTotal',
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$stockbalance',
                                        preserveNullAndEmptyArrays: false
                                    }
                                },

                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$codeExt',
                                            descricao: '$description',
                                            unidade: '$unidade',
                                            stockbalance: '$stockbalance'
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$item',
                            preserveNullAndEmptyArrays: true
                        }
                    },

                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                dateMov: '$dateMov',
                                quantidade: '$qty',
                                valor: '$totalValue',
                                stockvol: '$stockvol',
                                stocklot: '$stocklot',
                                stocklocal: '$stocklocal',
                                item: '$item'
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$stockmov',
                preserveNullAndEmptyArrays: false
            }
        },

        {
            $sort: { code: -1 }
        },
        {
            $replaceRoot: {
                newRoot: {
                    Id: { $ifNull: ['$_id','' ] },
                    CodigoOperacao: { $ifNull: ['$code', ''] },
                    DataReal: { $ifNull: ['$dateReal', ''] },
                    CodigoItem: { $ifNull: ['$stockmov.item.codigo','' ] },
                    DescricaoItem: { $ifNull: ['$stockmov.item.descricao','' ] },
                    DataMovimento: { $ifNull: ['$stockmov.dateMov', ''] },
                    Quantidade: { $ifNull: ['$stockmov.quantidade','' ] },
                    Valor: { $ifNull: ['$stockmov.valor','' ] },
                    Unidade: { $ifNull: ['$stockmov.item.unidade.descricao','' ] },
                    CodigoVolume: { $ifNull: ['$stockmov.stockvol.codigo', ''] },
                    QtdVolumes: { $ifNull: ['$stockmov.item.stockbalance.qtdVolumes', ''] },
                    CodigoLote: { $ifNull: ['$stockmov.stocklot.codigo', ''] },
                    CodigoLocal: { $ifNull: ['$stockmov.stocklocal.codigo', ''] },
                    DescricaoLocal: { $ifNull: ['$stockmov.stocklocal.descricao', ''] },
                }
            }
        },
    ]
}";


            query = query.Replace("<FilterIdOperacao>", model.IdOperacao);
            query = query.Replace("<FilterDate>", matchDate);
            query = query.Replace("<FilterCodigoOperacao>", model.CodigoOperacao);
            query = query.Replace("<FilterIdVol>", model.IdVolume);
            query = query.Replace("<FilterCodigoVol>", model.CodigoVolume);
            query = query.Replace("<FilterLoteId>", model.IdLote);
            query = query.Replace("<FilterLoteCodigo>", model.CodigoLote);
            query = query.Replace("<FilterLocalId>", model.IdLocal);
            query = query.Replace("<FilterLocalCodigo>", model.CodigoLocal);
            query = query.Replace("<FilterItemId>", model.IdItem);
            query = query.Replace("<FilterItemCodigo>", model.CodigoItem);
            return query;

        }
        #endregion

        #region fluxoentrada
        public static string GetFluxoEntrada(StockFilterEntradaModel model)
        {
            //string filterMatch = string.IsNullOrEmpty(filter) ?  : $@"{{ $match: {filter} }},";
            //string groupBy = $@"{{ $group: {{ '_id': '${groupByProperty}' , sum: {{ $sum: '$amount' }} }} }},";
            string filterDateInicio = model.DtInicio.HasValue ? model.DtInicio.ToString() : DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss");
            string filterDateFinal = model.DtFinal.HasValue ? model.DtFinal.ToString() : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss");

            string query = @"{
    aggregate: 'fisdocrec',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { 'isDeleted': false },
                    { 'numero': {$regex: '<FilterNumero>', $options: 'i'} },
                    { 'status': {$regex: '<FilterStatus>', $options: 'i'} },
                ]
            }
        },
        {
            $lookup: {
                from: 'account',
                localField: 'account',
                foreignField: '_id',
                as: 'account',
            }
        },
        {
            $unwind: {
                path: '$account',
                preserveNullAndEmptyArrays: true
            }
        },
        {
            $lookup: {
                from: 'stockoperation',
                localField: 'stockOperation',
                foreignField: '_id',
                as: 'operacaoDeEstoque',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { 'dateReal': {$gte:ISODate('<FilterDtInicio>'),$lte:ISODate('<FilterDtFinal>')}}
                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: 'stockmov',
                            localField: 'stockMovIds',
                            foreignField: '_id',
                            as: 'movimentosDeEstoque',
                            pipeline: [
                                {
                                    $lookup: {
                                        from: 'item',
                                        localField: 'itemId',
                                        foreignField: '_id',
                                        as: 'item',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { 'codeExt': {$regex: '<FilterItem>', $options: 'i'} },
                                                    ]
                                                }
                                            },
                                            {
                                                $lookup: {
                                                    from: 'unit',
                                                    localField: 'unitId',
                                                    foreignField: '_id',
                                                    as: 'unidade',
                                                    pipeline: [
                                                        {
                                                            $replaceRoot: {
                                                                newRoot: {
                                                                    id:'$_id',
                                                                    descricao:'$description',
                                                                }
                                                            }
                                                        }
                                                    ]
                                                }
                                            },
                                            {
                                                $unwind: {
                                                    path: '$unidade',
                                                    preserveNullAndEmptyArrays: true
                                                }
                                            },
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id:'$_id',
                                                        codigo:'$codeExt',
                                                        descricao:'$description',
                                                        unidade:'$unidade'
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$item',
                                        preserveNullAndEmptyArrays: true
                                    }
                                },
                                {
                                    $lookup: {
                                        from: 'stocklot',
                                        localField: 'stockLotId',
                                        foreignField: '_id',
                                        as: 'lote',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { code: {$regex: '<FilterLote>', $options: 'i'} },
                                                    ]
                                                }
                                            },
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id:'$_id',
                                                        codigo:'$code',
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$lote',
                                        preserveNullAndEmptyArrays: true
                                    }
                                },
                                {
                                    $lookup: {
                                        from: 'stockvol',
                                        localField: 'stockVolId',
                                        foreignField: '_id',
                                        as: 'volume',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { 'code': {$regex: '<FilterVolume>', $options: 'i'} },
                                                    ]
                                                }
                                            },
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id:'$_id',
                                                        codigo:'$code',
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$volume',
                                        preserveNullAndEmptyArrays: true
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id:'$_id',
                                            quantidadeRecebida:'$qty',
                                            validade:'$expirateDate',
                                            item:'$item',
                                            lote:'$lote',
                                            volume:'$volume'
                                        }
                                    }
                                },
                                {
                                    $sort:{validade: -1}    
                                }
                            ]
                        }
                    },
                    {
                        $lookup: {
                            from: 'stocklocal',
                            localField: 'stockLocalToId',
                            foreignField: '_id',
                            as: 'localDestino',
                            pipeline: [
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id:'$_id',
                                            descricao:'$description',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$localDestino',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id:'$_id',
                                codigo:'$code',
                                dataRealizacao: '$dateReal',
                                movimentosDeEstoque: '$movimentosDeEstoque',
                                localDestino: '$localDestino',
                            }
                        }
                    },
                ]
            }
        },
        {
            $unwind: {
                path: '$operacaoDeEstoque',
                preserveNullAndEmptyArrays: true
            }
        },
        {
            $sort:{numero: -1}    
        },
        {
            $unwind: {
                path : '$operacaoDeEstoque.movimentosDeEstoque',
                preserveNullAndEmptyArrays : true
            }
        },
        {
            $replaceRoot: {
                newRoot: {
                    NumeroNF: { $ifNull: ['$numero','']},
                    StatusNF: { $ifNull: ['$status','']},
                    DataNF: { $ifNull: ['$dataEmissao','']},
                    CodigoItem: { $ifNull: ['$operacaoDeEstoque.movimentosDeEstoque.item.codigo','']},
                    DescricaoItem: { $ifNull: ['$operacaoDeEstoque.movimentosDeEstoque.item.descricao','']},
                    UnidadeMedida: { $ifNull: ['$operacaoDeEstoque.movimentosDeEstoque.item.unidade.descricao','']},
                    QuantidadeRecebida: { $ifNull: ['$operacaoDeEstoque.movimentosDeEstoque.quantidadeRecebida','']},
                    CodigoDeOperacaoDoRecimento: { $ifNull: ['$operacaoDeEstoque.codigo','']},
                    DataDoRecebimento: { $ifNull: ['$operacaoDeEstoque.dataRealizacao','']},
                    LocalDestino: { $ifNull: ['$operacaoDeEstoque.localDestino.descricao','']},
                    Lote: { $ifNull: ['$operacaoDeEstoque.movimentosDeEstoque.lote.codigo','']},
                    Volume: { $ifNull: ['$operacaoDeEstoque.movimentosDeEstoque.volume.codigo','']},
                    Validade: { $ifNull: ['$operacaoDeEstoque.movimentosDeEstoque.validade','']},
                    CodFornecedor: { $ifNull: ['$account.code','']},
                    Fornecedor: { $ifNull: ['$account.name','']},
                }
            }
        },
    ]
}";
            query = query.Replace("<FilterNumero>", model.Nota);
            query = query.Replace("<FilterStatus>", model.Status);
            query = query.Replace("<FilterDtInicio>", filterDateInicio);
            query = query.Replace("<FilterDtFinal>", filterDateFinal);
            query = query.Replace("<FilterItem>", model.Item);
            query = query.Replace("<FilterVolume>", model.Volume);
            query = query.Replace("<FilterLote>", model.Lote);
            return query;
            //return query.Replace("<filter>", filterMatch);
            //            .Replace("<groupBy>", groupBy);
        }
        #endregion

        #region fluxosaida
        public static string GetFluxoSaida(StockFilterSaidaModel model)
        {
            //string filterMatch = string.IsNullOrEmpty(filter) ?  : $@"{{ $match: {filter} }},";
            //string match = @"{'picking.isDeleted': false},
            //        {'picking.codigo': {$regex: '<FilterPickingCodigo>', $options: 'i'}},
            //        {'picking.localDestDescription' : {$regex:  '<FilterLocalDestDescription>' , $options: 'i'}},
            //        {'picking.status' : {$regex: '<FilterPickingStatus>', $options: 'i'}},
            //        {'stockmov.stockLotCode' : {$regex: '<FilterStockLotCode>', $options: 'i'}},
            //        {'stockoperation.code' : {$regex: '<FilterStockOperationCode>', $options: 'i'}},
            //        {'stockoperation.status' : {$regex: '<FilterStockOperationStatus>', $options: 'i'}},
            //        {'item.codeExt' : {$regex: '<FilterItemCodeExt>' , $options:'i'}},
            //        {'pickingitem.descriptionItem' : {$regex: '<FilterDescriptionItem>', $options: 'i'}},
            //        {'predocemi.status_carga' : {$regex: '<FilterStatusCarga>', $options: 'i'}},
            //        {'predocemi.status' : {$regex: '<FilterStatus>', $options: 'i'}},";


            //if (!string.IsNullOrEmpty(model.ConfirmacaoEntregaStatus) && !string.IsNullOrEmpty(model.PickingNotaFiscal))
            //{
            //    match += @"{'picking.notaFiscal' : {$regex: '<FilterNotaFiscal>' , $options: 'i'}},
            //               {'confirmacaoentrega.status' : {$regex: '<FilterConfirmacaoEntregaStatus>', $options: 'i'}},";
            //}
            //else if (!string.IsNullOrEmpty(model.ConfirmacaoEntregaStatus) && string.IsNullOrEmpty(model.PickingNotaFiscal))
            //{
            //    match += @"{'confirmacaoentrega.status' : {$regex: '<FilterConfirmacaoEntregaStatus>', $options: 'i'}},";
            //}
            //else if (string.IsNullOrEmpty(model.ConfirmacaoEntregaStatus) && !string.IsNullOrEmpty(model.PickingNotaFiscal))
            //{
            //    match += @"{'picking.notaFiscal' : {$regex: '<FilterNotaFiscal>' , $options: 'i'}},";
            //}

            string matchDate = "";
            if (model.PickingDtInicio != null && model.PickingDtFinal != null)
            {
                matchDate = @"{ 'datePicking': {$gte:ISODate('<FilterDtInicio>'),$lte:ISODate('<FilterDtFinal>')}}";
                matchDate = matchDate.Replace("<FilterDtInicio>", model.PickingDtInicio.ToString());
                matchDate = matchDate.Replace("<FilterDtFinal>", model.PickingDtFinal.ToString());
            }
            string query = @"{
    aggregate: 'picking',
    cursor: { 'batchSize': 100000000 },
    pipeline: [
        {
            $match: {
                $and: [
                    { isDeleted: false },
                    { notaFiscal: { $regex: '<FilterPickingNotaFiscal>', $options: 'i' } },
                    { codigo: { $regex: '<FilterPickingCodigo>', $options: 'i' } },
                    { status: { $regex: '<FilterPickingStatus>', $options: 'i' } },
                    <FilterDate>
                ]
            }
        },
        {
            $lookup: {
                from: 'pickingitem',
                localField: 'itemsPicking',
                foreignField: '_id',
                as: 'pickingitem',
                pipeline: [
                    {
                        $lookup: {
                            from: 'item',
                            localField: 'itemId',
                            foreignField: '_id',
                            as: 'item',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { isDeleted: false },
                                            { codeExt: { $regex: '<FilterItemCodigo>', $options: 'i' } },
                                            { description: { $regex: '<FilterItemDescription>', $options: 'i' } },

                                        ]
                                    }
                                },
                                {
                                    $lookup: {
                                        from: 'unit',
                                        localField: 'unitId',
                                        foreignField: '_id',
                                        as: 'unidade',
                                        pipeline: [
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id: '$_id',
                                                        descricao: '$description',
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$unidade',
                                        preserveNullAndEmptyArrays: true
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            codigo: '$codeExt',
                                            descricao: '$description',
                                            unidade: '$unidade'
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$item',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $lookup: {
                            from: 'stockmov',
                            localField: 'stockMovIds',
                            foreignField: '_id',
                            as: 'stockmov',
                            pipeline: [
                                {
                                    $lookup: {
                                        from: 'stocklot',
                                        localField: 'stockLotId',
                                        foreignField: '_id',
                                        as: 'stocklot',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { isDeleted: false },
                                                        { code: { $regex: '<FilterLoteCodigo>', $options: 'i' } },
                                                    ]
                                                }
                                            },
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id: '$_id',
                                                        codigo: '$code'
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$stocklot',
                                        preserveNullAndEmptyArrays: true
                                    }
                                },

                                {
                                    $lookup: {
                                        from: 'stockvol',
                                        localField: 'stockVolId',
                                        foreignField: '_id',
                                        as: 'stockvol',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { isDeleted: false },
                                                        { code: { $regex: '<FilterVolumeCodigo>', $options: 'i' } },
                                                    ]
                                                }
                                            },
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id: '$_id',
                                                        codigo: '$code'
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$stockvol',
                                        preserveNullAndEmptyArrays: true
                                    }
                                },

                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            stocklot: '$stocklot',
                                            stockvol: '$stockvol',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$stockmov',
                            preserveNullAndEmptyArrays: true
                        }
                    },


                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                qtd: '$qtd',
                                qtdSeparate: '$qtdSeparate',
                                stockmov: '$stockmov',
                                item: '$item',
                                createBy: '$createBy',
                                createAt : '$createAt'
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$pickingitem',
                preserveNullAndEmptyArrays: true
            }
        },
        {
            $lookup: {
                from: 'stocklocal',
                localField: 'localDest',
                foreignField: '_id',
                as: 'stocklocal',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { isDeleted: false },
                                { description: { $regex: '<FilterLocalDescription>', $options: 'i' } },

                            ]
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                descricao: '$description',
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$stocklocal',
                preserveNullAndEmptyArrays: true
            }
        },

        {
            $lookup: {
                from: 'stockoperation',
                localField: 'operationStockId',
                foreignField: '_id',
                as: 'stockoperation',
                pipeline: [
                    {
                        $match: {
                            $and: [
                                { isDeleted: false },
                                { code: { $regex: '<FilterStockOperationCode>', $options: 'i' } },

                            ]
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                codigo: '$code',
                                status: '$status',
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$stockoperation',
                preserveNullAndEmptyArrays: true
            }
        },
        {
            $lookup: {
                from: 'salesorder',
                localField: 'codigo',
                foreignField: 'code',
                as: 'salesorder',
                pipeline: [
                    {
                        $lookup: {
                            from: 'stocklocal',
                            localField: 'localDest',
                            foreignField: '_id',
                            as: 'stocklocal',
                            pipeline: [
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            descricao: '$description',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$stocklocal',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $lookup: {
                            from: 'predocemi',
                            localField: 'code',
                            foreignField: 'chave_acesso',
                            as: 'predocemi',
                            pipeline: [
                                {
                                    $match: {
                                        $and: [
                                            { isDeleted: false },
                                            { status: { $regex: '<FilterPredocemiStatus>', $options: 'i' } },
                                        ]
                                    }
                                },
                                {
                                    $lookup: {
                                        from: 'confirmacaoentrega',
                                        localField: 'chave_acesso',
                                        foreignField: 'nf',
                                        as: 'confirmacaoentrega',
                                        pipeline: [
                                            {
                                                $match: {
                                                    $and: [
                                                        { isDeleted: false },
                                                        { status: { $regex: '<FilterStatusEntrega>', $options: 'i' } },
                                                  
                                                    ]
                                                }
                                            },
                                            {
                                                $replaceRoot: {
                                                    newRoot: {
                                                        id: '$_id',
                                                        status: '$status',
                                                        dataEntrega: '$data_confirmacao',
                                                        colaborador: '$usuario_confirmacao',
                                                    }
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    $unwind: {
                                        path: '$confirmacaoentrega',
                                        preserveNullAndEmptyArrays: true
                                    }
                                },
                                {
                                    $replaceRoot: {
                                        newRoot: {
                                            id: '$_id',
                                            dataEmissao: '$dataEmissao',
                                            status_carga: '$status_carga',
                                            status: '$status',
                                            confirmacaoentrega: '$confirmacaoentrega',
                                        }
                                    }
                                }
                            ]
                        }
                    },
                    {
                        $unwind: {
                            path: '$predocemi',
                            preserveNullAndEmptyArrays: true
                        }
                    },
                    {
                        $replaceRoot: {
                            newRoot: {
                                id: '$_id',
                                stocklocal: '$stocklocal',
                                predocemi: '$predocemi',
                            }
                        }
                    }
                ]
            }
        },
        {
            $unwind: {
                path: '$salesorder',
                preserveNullAndEmptyArrays: true
            }
        },

        {
            $sort: { codigo: -1 }
        },

        {
            $replaceRoot: {
                newRoot: {
                    PickingCodigo: { $ifNull: ['$codigo', ''] },
                    StatusPicking: { $ifNull: ['$status', ''] },
                    PickingInclusao: { $ifNull: ['$datePicking', ''] },
                    LocalDestino: { $ifNull: ['$stocklocal.descricao', ''] },
                    CodigoOperacao: { $ifNull: ['$stockoperation.codigo', ''] },
                    StatusOperacao: { $ifNull: ['$stockoperation.status', ''] },
                    ItemCodigo: { $ifNull: ['$pickingitem.item.codigo', ''] },
                    ItemDescription: { $ifNull: ['$pickingitem.item.descricao', ''] },
                    UnidadeItem: { $ifNull: ['$pickingitem.item.unidade.descricao', ''] },
                    Lote: { $ifNull: ['$pickingitem.stockmov.stocklot.codigo', ''] },
                    Volume: { $ifNull: ['$pickingitem.stockmov.stockvol.codigo', ''] },
                    Remessa: { $ifNull: ['$remessa', ''] },
                    Confirmado: { $ifNull: ['$confirmado', ''] },
                    NFSaida: { $ifNull: ['$notaFiscal', ''] },
                    DestinatarioNF: { $ifNull: ['$salesorder.stocklocal.descricao', ''] },
                    DataEmissao: { $ifNull: ['$salesorder.predocemi.dataEmissao', ''] },
                    StatusNF: { $ifNull: ['$salesorder.predocemi.status_carga', ''] },
                    StatusColeta: { $ifNull: ['$salesorder.predocemi.status', ''] },
                    StatusEntrega: { $ifNull: ['$salesorder.predocemi.confirmacaoentrega.status', ''] },
                    DataHoraEntrega: { $ifNull: ['$salesorder.predocemi.confirmacaoentrega.dataEntrega', ''] },
                    ColaboradorPicking: { $ifNull: ['$createBy', ''] },
                    ColaboradorPickingItem: { $ifNull: ['$pickingitem.createBy', ''] },
                    QuantidadeSolicitada: { $ifNull: ['$pickingitem.qtd', ''] },
                    QuantidadePicking: { $ifNull: ['$pickingitem.qtdSeparate', ''] },
                    QuantidadeEnviada: { $ifNull: ['$pickingitem.qtdSeparate', ''] },
                    UsuarioInicioPicking: { $ifNull: ['$userInitPicking', ''] },
                    DataInicioPicking: { $ifNull: ['$dateInitPicking', ''] },
                    UsuarioFimPicking: { $ifNull: ['$userFinished', ''] },
                    DataFimPicking: { $ifNull: ['$dateFinished', ''] },
                    DataPickingItem: { $ifNull: ['$pickingitem.createAt', ''] },
                }
            }
        },
    ]
}";
            //            query = query.Replace("<Match>", match);
            query = query.Replace("<FilterPickingNotaFiscal>", model.PickingNotaFiscal);
            query = query.Replace("<FilterPickingCodigo>", model.PickingCodigo);
            query = query.Replace("<FilterPickingStatus>", model.PickingStatus);
            query = query.Replace("<FilterItemCodigo>", model.ItemCodigo);
            query = query.Replace("<FilterItemDescription>", model.ItemDescription);
            query = query.Replace("<FilterLoteCodigo>", model.StockLotCode);
            query = query.Replace("<FilterVolumeCodigo>", model.StockVolCode);
            query = query.Replace("<FilterLocalDescription>", model.LocalDescription);
            query = query.Replace("<FilterStockOperationCode>", model.StockOperationCode);
            query = query.Replace("<FilterPredocemiStatus>", model.Status);
            query = query.Replace("<FilterStatusEntrega>", model.StatusEntrega);
            query = query.Replace("<FilterDate>", matchDate);

            return query;
            //            .Replace("<groupBy>", groupBy);
        }
        #endregion


        #region getSaldo


        public static string GetAllSaldoReset()
        {
            string query = @"{
  aggregate: 'stockvolcont',
  cursor: {'batchSize': 100000},
  pipeline: [
    {
      $match: {
        quantidade: { $gt: 0 },
        isDeleted: false
      }
    },
    {
      $lookup: {
        from: 'stockvol',
        localField: '_id',
        foreignField: 'stockVolCont',
        as: 'stockvol_docs'
      }
    },
    {
      $unwind: '$stockvol_docs'
    },
    {
      $match: {
        'stockvol_docs.isDeleted': false
      }
    },
    {
      $lookup: {
        from: 'stocklocal',
        localField: 'stockvol_docs.stockLocalId',
        foreignField: '_id',
        as: 'stocklocal'
      }
    },
    {
      $unwind: '$stocklocal'
    },
    {
      $match: {
        'stocklocal.isDeleted': false
      }
    },
    {
      $lookup: {
        from: 'item',
        localField: 'idItem',
        foreignField: '_id',
        as: 'item_docs'
      }
    },
    {
      $unwind: '$item_docs'
    },
    {
      $lookup: {
        from: 'stocklot',
        localField: 'idLote',
        foreignField: '_id',
        as: 'stocklot'
      }
    },
    {
      $unwind: '$stocklot'
    },
    {
      $group: {
        _id: {
          idItem: '$idItem',
          idLocal: '$stockvol_docs.stockLocalId',
          idLote: '$idLote'
        },
        totalQuantidade: { $sum: '$quantidade' },
        itemCode: { $first: '$item_docs.codeExt' },
        itemDescription: { $first: '$item_docs.description' },
        unidade: { $first: '$item_docs.unitName' },
        code_Lote: { $first: '$stocklot.code' },
        validadeLote: { $first: '$stocklot.expirateDate' },
        stockLocalCode: { $first: '$stockvol_docs.stockLocalCode' }
      }
    },
    {
      $project: {
        idItem: '$_id.idItem',
        idLocal: '$_id.idLocal',
        idLote: '$_id.idLote',
        itemCode: 1,
        itemDescription: 1,
        unidade: 1,
        code_Lote: 1,
        validadeLote: 1,
        stockLocalCode: 1,
        totalQuantidade: 1
      }
    }
  ]
}";



            return query;

        }


        #endregion
    }
}
