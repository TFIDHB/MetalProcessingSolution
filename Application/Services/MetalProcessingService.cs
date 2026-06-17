using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class MetalProcessingService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService storageService) : IMetalProcessingService
    {
        public async Task<IEnumerable<MetalServiceResponseDto>> GetServicesAsync(CancellationToken cancellationToken)
        {
            var services = await unitOfWork.MetalServices.GetAllAsync(cancellationToken);
            return mapper.Map<IEnumerable<MetalServiceResponseDto>>(services);
        }

        public async Task<int> CreateServiceAsync(MetalServiceDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || dto.PriceFrom <= 0)
                throw new AppValidationException("Название услуги обязательно, а цена должна быть больше нуля.");

            var service = mapper.Map<MetalService>(dto);

            if (dto.ImageFile != null)
            {
                service.ImageUrl = await storageService.SaveFileAsync(dto.ImageFile, "services", cancellationToken);
            }

            await unitOfWork.MetalServices.AddAsync(service, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return service.Id;
        }

        public async Task UpdateServiceAsync(int id, MetalServiceDto dto, CancellationToken cancellationToken)
        {
            var service = await unitOfWork.MetalServices.GetByIdAsync(id, cancellationToken);
            if (service == null) throw new NotFoundException("Услуга", id);

            mapper.Map(dto, service);

            if (dto.ImageFile != null)
            {
                storageService.DeleteFile(service.ImageUrl);
                service.ImageUrl = await storageService.SaveFileAsync(dto.ImageFile, "services", cancellationToken);
            }

            unitOfWork.MetalServices.Update(service);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteServiceAsync(int id, CancellationToken cancellationToken)
        {
            var service = await unitOfWork.MetalServices.GetByIdAsync(id, cancellationToken);
            if (service == null) throw new NotFoundException("Услуга", id);

            storageService.DeleteFile(service.ImageUrl);
            unitOfWork.MetalServices.Delete(service);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        public async Task<IEnumerable<UnliquidProduct>> GetUnliquidProductsAsync(CancellationToken cancellationToken)
        {
            return await unitOfWork.UnliquidProducts.GetAllAsync(cancellationToken);
        }

        public async Task<int> CreateUnliquidAsync(UnliquidProductDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0)
                throw new AppValidationException("Название товара обязательно, а цена должна быть выше нуля.");

            var product = mapper.Map<UnliquidProduct>(dto);

            if (dto.ImageFile != null)
            {
                product.ImageUrl = await storageService.SaveFileAsync(dto.ImageFile, "products", cancellationToken);
            }

            await unitOfWork.UnliquidProducts.AddAsync(product, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

        public async Task UpdateUnliquidAsync(int id, UnliquidProductDto dto, CancellationToken cancellationToken)
        {
            var product = await unitOfWork.UnliquidProducts.GetByIdAsync(id, cancellationToken);
            if (product == null) throw new NotFoundException("Неликвидный товар", id);

            mapper.Map(dto, product);

            if (dto.ImageFile != null)
            {
                storageService.DeleteFile(product.ImageUrl);
                product.ImageUrl = await storageService.SaveFileAsync(dto.ImageFile, "products", cancellationToken);
            }

            unitOfWork.UnliquidProducts.Update(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteUnliquidAsync(int id, CancellationToken cancellationToken)
        {
            var product = await unitOfWork.UnliquidProducts.GetByIdAsync(id, cancellationToken);
            if (product == null) throw new NotFoundException("Неликвидный товар", id);

            storageService.DeleteFile(product.ImageUrl);
            unitOfWork.UnliquidProducts.Delete(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}