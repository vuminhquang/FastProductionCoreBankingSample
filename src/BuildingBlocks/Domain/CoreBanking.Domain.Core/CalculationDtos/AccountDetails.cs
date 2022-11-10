﻿using CoreBanking.Domain.Core.Models;

namespace CoreBanking.Domain.Core.CalculationDtos;

public record AccountDetails(Guid Id, Guid OwnerId, string OwnerFirstName, string OwnerLastName, string OwnerEmail, Money Balance);