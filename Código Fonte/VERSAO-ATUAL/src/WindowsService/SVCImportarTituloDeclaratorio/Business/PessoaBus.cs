using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
    public class PessoaBus
    {
        PessoaDa _da = new PessoaDa();

        GerenciadorConfiguracao<ConfiguracaoPessoa> _configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());

        public Pessoa Obter(String cpfCnpj, BancoDeDados banco, bool simplificado = false)
        {
            Pessoa pessoa = _da.Obter(cpfCnpj, banco, simplificado);

            if (pessoa != null && pessoa.Id > 0 && pessoa.IsFisica)
            {
                if ((pessoa.Fisica.EstadoCivil ?? 0) > 0)
                {
                    pessoa.Fisica.EstadoCivilTexto = _configPessoa.Obter<List<EstadoCivil>>(ConfiguracaoPessoa.KeyEstadosCivis).Single(x => x.Id == pessoa.Fisica.EstadoCivil).Texto;
                }
                if ((pessoa.Fisica.Sexo ?? 0) > 0)
                {
                    pessoa.Fisica.SexoTexto = _configPessoa.Obter<List<Sexo>>(ConfiguracaoPessoa.KeySexos).Single(x => x.Id == pessoa.Fisica.Sexo).Texto;
                }
            }

            return pessoa;
        }

        public void AlterarConjuge(int id, BancoDeDados banco)
        {
            _da.AlterarConjuge(id, banco);
        }

        public int ObterId(String cpfCnpj, BancoDeDados banco)
        {
            return _da.ObterId(cpfCnpj, banco);

            return 0;
        }

        public Pessoa Importar(Pessoa pessoa, BancoDeDados banco, bool conjuge = false)
        {   
                int id = pessoa.Id;
                int conj = (pessoa.IsFisica) ? pessoa.Fisica.ConjugeId.GetValueOrDefault() : 0;
                PessoaCredenciadoBus _busPessoaCredenciado = new PessoaCredenciadoBus();
                List<Pessoa> representantes = pessoa.Juridica.Representantes;
                pessoa = _busPessoaCredenciado.Obter(pessoa.Id);
                pessoa.Juridica.Representantes = representantes;
                pessoa.Id = ObterId(pessoa.CPFCNPJ, banco);
                pessoa.Endereco.Id = 0;
                pessoa.Fisica.ConjugeId = conj;

                if (conjuge)
                {
                    conj = pessoa.Fisica.ConjugeId.GetValueOrDefault();
                    pessoa.Fisica.ConjugeId = 0;
                }

                if (pessoa.Id == 0)
                {
                    pessoa.Fisica.ConjugeId = 0;
                    _da.Criar(pessoa, banco);
                }
                else
                {
                    _da.Editar(pessoa, banco);
                }

                if (conjuge)
                {
                    pessoa.Fisica.ConjugeId = conj;
                }

                pessoa.InternoId = pessoa.Id;
                pessoa.Id = id;
            
            return pessoa;
        }

        public void AlterarConjugeEstadoCivil(int id, int conjuge, BancoDeDados banco)
        {
            _da.AlterarConjugeEstadoCivil(id, conjuge, banco);
        }
    }
}
