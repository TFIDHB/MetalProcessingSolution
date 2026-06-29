using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class MetalProcessingService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService storageService) : IMetalProcessingService
{
    public async Task<IEnumerable<UnliquidProductResponseDto>> GetUnliquidProductsAsync(CancellationToken cancellationToken)
    {
        var products = await unitOfWork.UnliquidProducts.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<UnliquidProductResponseDto>>(products);
    }

    public async Task<int> CreateUnliquidAsync(UnliquidProductDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0)
            throw new AppValidationException("Название товара обязательно, а цена должна быть выше нуля.");

        var product = mapper.Map<UnliquidProduct>(dto);

        if (dto.NewImages != null)
        {
            foreach (var file in dto.NewImages)
            {
                var url = await storageService.SaveFileAsync(file, "products", cancellationToken);
                product.Images.Add(new UnliquidProductImage { ImageUrl = url });
            }
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

        if (dto.DeleteImageIds != null && dto.DeleteImageIds.Count > 0)
        {
            var imagesToRemove = product.Images.Where(i => dto.DeleteImageIds.Contains(i.Id)).ToList();
            foreach (var img in imagesToRemove)
            {
                storageService.DeleteFile(img.ImageUrl);
                product.Images.Remove(img);
            }
        }

        if (dto.NewImages != null)
        {
            foreach (var file in dto.NewImages)
            {
                var url = await storageService.SaveFileAsync(file, "products", cancellationToken);
                product.Images.Add(new UnliquidProductImage { ImageUrl = url });
            }
        }

        unitOfWork.UnliquidProducts.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUnliquidAsync(int id, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.UnliquidProducts.GetByIdAsync(id, cancellationToken);
        if (product == null) throw new NotFoundException("Неликвидный товар", id);

        foreach (var img in product.Images)
        {
            storageService.DeleteFile(img.ImageUrl);
        }

        unitOfWork.UnliquidProducts.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

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

        if (dto.NewImages != null)
        {
            foreach (var file in dto.NewImages)
            {
                var url = await storageService.SaveFileAsync(file, "services", cancellationToken);
                service.Images.Add(new MetalServiceImage { ImageUrl = url });
            }
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

        if (dto.DeleteImageIds != null && dto.DeleteImageIds.Count > 0)
        {
            var imagesToRemove = service.Images.Where(i => dto.DeleteImageIds.Contains(i.Id)).ToList();
            foreach (var img in imagesToRemove)
            {
                storageService.DeleteFile(img.ImageUrl);
                service.Images.Remove(img);
            }
        }

        if (dto.NewImages != null)
        {
            foreach (var file in dto.NewImages)
            {
                var url = await storageService.SaveFileAsync(file, "services", cancellationToken);
                service.Images.Add(new MetalServiceImage { ImageUrl = url });
            }
        }

        unitOfWork.MetalServices.Update(service);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteServiceAsync(int id, CancellationToken cancellationToken)
    {
        var service = await unitOfWork.MetalServices.GetByIdAsync(id, cancellationToken);
        if (service == null) throw new NotFoundException("Услуга", id);

        foreach (var img in service.Images)
        {
            storageService.DeleteFile(img.ImageUrl);
        }

        unitOfWork.MetalServices.Delete(service);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AdjustServicePricesAsync(PriceAdjustmentDto dto, CancellationToken cancellationToken)
    {
        if (dto.Percent == 0)
            throw new AppValidationException("Процент не может быть равен нулю.");

        if (dto.Percent <= -100)
            throw new AppValidationException("Нельзя снизить цену на 100% и более.");

        var services = await unitOfWork.MetalServices.GetAllAsync(cancellationToken);
        var multiplier = 1 + dto.Percent / 100m;

        foreach (var service in services)
        {
            service.PriceFrom = Math.Round(service.PriceFrom * multiplier, 2);
        }

        foreach (var service in services)
            unitOfWork.MetalServices.Update(service);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AdjustUnliquidPricesAsync(PriceAdjustmentDto dto, CancellationToken cancellationToken)
    {
        if (dto.Percent == 0)
            throw new AppValidationException("Процент не может быть равен нулю.");

        if (dto.Percent <= -100)
            throw new AppValidationException("Нельзя снизить цену на 100% и более.");

        var products = await unitOfWork.UnliquidProducts.GetAllAsync(cancellationToken);
        var multiplier = 1 + dto.Percent / 100m;

        foreach (var product in products)
        {
            product.Price = Math.Round(product.Price * multiplier, 2);
        }

        foreach (var product in products)
            unitOfWork.UnliquidProducts.Update(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var services = await unitOfWork.MetalServices.GetAllAsync(cancellationToken);
        var unliquids = await unitOfWork.UnliquidProducts.GetAllAsync(cancellationToken);

        return new AdminStatsDto
        {
            ServicesCount = services.Count(),
            UnliquidsCount = unliquids.Count(),
            TotalUnliquidsValue = unliquids.Sum(p =>
            {
                var digits = new string(p.Quantity.Where(char.IsDigit).ToArray());
                return int.TryParse(digits, out var qty) ? p.Price * qty : p.Price;
            })
        };
    }
}