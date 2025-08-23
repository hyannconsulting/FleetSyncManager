using FluentValidation;
using Laroche.FleetManager.Application.Commands.Vehicles;

namespace Laroche.FleetManager.Application.Validators.Vehicles;

/// <summary>
/// Validator for CreateVehicleCommand
/// </summary>
public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    /// <summary>
    /// Initializes the validator
    /// </summary>
    public CreateVehicleCommandValidator()
    {
        RuleFor(v => v.LicensePlate)
            .NotEmpty().WithMessage("Le numéro d'immatriculation est requis")
            .MaximumLength(20).WithMessage("Le numéro d'immatriculation ne peut pas dépasser 20 caractères")
            .Matches(@"^[A-Z0-9-]+$").WithMessage("Le numéro d'immatriculation ne doit contenir que des lettres majuscules, des chiffres et des tirets");

        RuleFor(v => v.Vin)
            .MaximumLength(17).WithMessage("Le numéro VIN ne peut pas dépasser 17 caractères")
            .When(v => !string.IsNullOrEmpty(v.Vin));

        RuleFor(v => v.Brand)
            .NotEmpty().WithMessage("La marque est requise")
            .MaximumLength(50).WithMessage("La marque ne peut pas dépasser 50 caractères");

        RuleFor(v => v.Model)
            .NotEmpty().WithMessage("Le modèle est requis")
            .MaximumLength(50).WithMessage("Le modèle ne peut pas dépasser 50 caractères");

        RuleFor(v => v.Year)
            .GreaterThan(1950).WithMessage("L'année doit être supérieure à 1950")
            .LessThanOrEqualTo(DateTime.Now.Year + 2).WithMessage($"L'année ne peut pas dépasser {DateTime.Now.Year + 2}");

        RuleFor(v => v.FuelType)
            .NotEmpty().WithMessage("Le type de carburant est requis");

        RuleFor(v => v.CurrentMileage)
            .GreaterThanOrEqualTo(0).WithMessage("Le kilométrage doit être positif ou nul");

        RuleFor(v => v.PurchasePrice)
            .GreaterThan(0).WithMessage("Le prix d'achat doit être positif")
            .When(v => v.PurchasePrice.HasValue);
    }
}

/// <summary>
/// Validator for UpdateVehicleCommand
/// </summary>
public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    /// <summary>
    /// Initializes the validator
    /// </summary>
    public UpdateVehicleCommandValidator()
    {
        RuleFor(v => v.Id)
            .GreaterThan(0).WithMessage("L'ID du véhicule doit être positif");

        RuleFor(v => v.LicensePlate)
            .NotEmpty().WithMessage("Le numéro d'immatriculation est requis")
            .MaximumLength(20).WithMessage("Le numéro d'immatriculation ne peut pas dépasser 20 caractères")
            .Matches(@"^[A-Z0-9-]+$").WithMessage("Le numéro d'immatriculation ne doit contenir que des lettres majuscules, des chiffres et des tirets");

        RuleFor(v => v.Vin)
            .MaximumLength(17).WithMessage("Le numéro VIN ne peut pas dépasser 17 caractères")
            .When(v => !string.IsNullOrEmpty(v.Vin));

        RuleFor(v => v.Brand)
            .NotEmpty().WithMessage("La marque est requise")
            .MaximumLength(50).WithMessage("La marque ne peut pas dépasser 50 caractères");

        RuleFor(v => v.Model)
            .NotEmpty().WithMessage("Le modèle est requis")
            .MaximumLength(50).WithMessage("Le modèle ne peut pas dépasser 50 caractères");

        RuleFor(v => v.Year)
            .GreaterThan(1950).WithMessage("L'année doit être supérieure à 1950")
            .LessThanOrEqualTo(DateTime.Now.Year + 2).WithMessage($"L'année ne peut pas dépasser {DateTime.Now.Year + 2}");

        RuleFor(v => v.FuelType)
            .NotEmpty().WithMessage("Le type de carburant est requis");

        RuleFor(v => v.CurrentMileage)
            .GreaterThanOrEqualTo(0).WithMessage("Le kilométrage doit être positif ou nul");

        RuleFor(v => v.Status)
            .NotEmpty().WithMessage("Le statut est requis");
    }
}

/// <summary>
/// Validator for DeleteVehicleCommand
/// </summary>
public class DeleteVehicleCommandValidator : AbstractValidator<DeleteVehicleCommand>
{
    /// <summary>
    /// Initializes the validator
    /// </summary>
    public DeleteVehicleCommandValidator()
    {
        RuleFor(v => v.Id)
            .GreaterThan(0).WithMessage("L'ID du véhicule doit être positif");
    }
}
