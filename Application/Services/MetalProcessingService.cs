using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class MetalProcessingService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService storageService) : IMetalProcessingService
    {
        public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<ProductCategory>(category, true, out var cat))
                throw new AppValidationException($"Неизвестная категория: {category}");

            var products = await unitOfWork.Products.GetByCategoryAsync(cat, cancellationToken);
            return mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<int> CreateProductAsync(ProductDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0)
                throw new AppValidationException("Название товара обязательно, а цена должна быть выше нуля.");

            if (!Enum.TryParse<ProductCategory>(dto.Category, true, out var cat))
                throw new AppValidationException($"Неизвестная категория: {dto.Category}");

            var product = mapper.Map<Product>(dto);
            product.Category = cat;

            if (dto.NewImages != null)
            {
                foreach (var file in dto.NewImages)
                {
                    var url = await storageService.SaveFileAsync(file, "products", cancellationToken);
                    product.Images.Add(new ProductImage { ImageUrl = url });
                }
            }

            await unitOfWork.Products.AddAsync(product, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

        public async Task UpdateProductAsync(int id, ProductDto dto, CancellationToken cancellationToken)
        {
            var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
            if (product == null) throw new NotFoundException("Товар", id);

            mapper.Map(dto, product);

            if (!string.IsNullOrWhiteSpace(dto.Category) &&
                Enum.TryParse<ProductCategory>(dto.Category, true, out var cat))
                product.Category = cat;

            if (dto.DeleteImageIds != null && dto.DeleteImageIds.Count > 0)
            {
                var toRemove = product.Images.Where(i => dto.DeleteImageIds.Contains(i.Id)).ToList();
                foreach (var img in toRemove)
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
                    product.Images.Add(new ProductImage { ImageUrl = url });
                }
            }

            unitOfWork.Products.Update(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
            if (product == null) throw new NotFoundException("Товар", id);

            foreach (var img in product.Images)
                storageService.DeleteFile(img.ImageUrl);

            unitOfWork.Products.Delete(product);
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
                var toRemove = service.Images.Where(i => dto.DeleteImageIds.Contains(i.Id)).ToList();
                foreach (var img in toRemove)
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
                storageService.DeleteFile(img.ImageUrl);

            unitOfWork.MetalServices.Delete(service);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task AdjustServicePricesAsync(PriceAdjustmentDto dto, CancellationToken cancellationToken)
        {
            if (dto.Percent == 0) throw new AppValidationException("Процент не может быть равен нулю.");
            if (dto.Percent <= -100) throw new AppValidationException("Нельзя снизить цену на 100% и более.");

            var services = await unitOfWork.MetalServices.GetAllAsync(cancellationToken);
            var multiplier = 1 + dto.Percent / 100m;

            foreach (var service in services)
            {
                service.PriceFrom = Math.Round(service.PriceFrom * multiplier, 2);
                unitOfWork.MetalServices.Update(service);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task AdjustProductPricesAsync(string category, PriceAdjustmentDto dto, CancellationToken cancellationToken)
        {
            if (dto.Percent == 0) throw new AppValidationException("Процент не может быть равен нулю.");
            if (dto.Percent <= -100) throw new AppValidationException("Нельзя снизить цену на 100% и более.");

            if (!Enum.TryParse<ProductCategory>(category, true, out var cat))
                throw new AppValidationException($"Неизвестная категория: {category}");

            var products = await unitOfWork.Products.GetByCategoryAsync(cat, cancellationToken);
            var multiplier = 1 + dto.Percent / 100m;

            foreach (var product in products)
            {
                product.Price = Math.Round(product.Price * multiplier, 2);
                unitOfWork.Products.Update(product);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken)
        {
            var services = await unitOfWork.MetalServices.GetAllAsync(cancellationToken);
            var unliquids = await unitOfWork.Products.GetByCategoryAsync(ProductCategory.Unliquid, cancellationToken);
            var ourProducts = await unitOfWork.Products.GetByCategoryAsync(ProductCategory.OurProducts, cancellationToken);
            var generalGoods = await unitOfWork.Products.GetByCategoryAsync(ProductCategory.GeneralGoods, cancellationToken);

            decimal CalcValue(IEnumerable<Product> products) => products.Sum(p =>
            {
                var digits = new string(p.Quantity.Where(char.IsDigit).ToArray());
                return int.TryParse(digits, out var qty) ? p.Price * qty : p.Price;
            });

            return new AdminStatsDto
            {
                ServicesCount = services.Count(),
                UnliquidsCount = unliquids.Count(),
                OurProductsCount = ourProducts.Count(),
                GeneralGoodsCount = generalGoods.Count(),
                TotalUnliquidsValue = CalcValue(unliquids),
                TotalOurProductsValue = CalcValue(ourProducts),
                TotalGeneralGoodsValue = CalcValue(generalGoods)
            };
        }
    }
}