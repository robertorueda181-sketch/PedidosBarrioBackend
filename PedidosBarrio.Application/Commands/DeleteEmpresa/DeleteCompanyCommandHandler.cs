using MediatR;
using PedidosBarrio.Domain.Repositories;


namespace PedidosBarrio.Application.Commands.DeleteEmpresa
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Unit>
    {
        private readonly ICompanyRepository _companyRepository; // IEmpresaRepository -> ICompanyRepository

        public DeleteCompanyCommandHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Unit> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(command.Id); // empresa -> company
            if (company == null)
            {
                throw new ApplicationException($"Company with ID {command.Id} not found."); // Empresa -> Company
            }

            await _companyRepository.DeleteAsync(command.Id);
            return Unit.Value;
        }
    }
}