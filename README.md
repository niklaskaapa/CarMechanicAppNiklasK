# Examination - CarMechanic Application

**Elev:** Niklas Kääpä
**Datum:** 2026-04-16
**Domän:** CarMechanic

## Instruktioner

Du har fått en Console-applikation i **Onion Architecture** som innehåller buggar och
saknade metodimplementationer. Din uppgift är att:

1. **Felsöka och fixa 8 buggar** i Application- och Infrastructure-lagren
2. **Implementera 4 saknade metoder** (markerade med `NotImplementedException`)

## Regler

- Du får **INTE** ändra namn på klasser, metoder, namespaces eller interfaces
- Du får **INTE** lägga till nya projekt eller ändra projektstrukturen
- Du får **INTE** ändra i testprojektet
- Alla metodsignaturer måste vara exakt som de är (parametrar, returtyper)
- Följ Onion Architecture-principerna (beroenden pekar inåt)

## Projektstruktur

```
CarMechanicApp/
├── CarMechanic.Domain/       # Entiteter, enums, interfaces (ÄNDRA INTE)
├── CarMechanic.Application/  # Services - HÄR FIXAR DU BUGGAR & IMPLEMENTERAR METODER
├── CarMechanic.Infrastructure/ # Repositories & factories - KAN INNEHÅLLA BUGGAR
└── CarMechanic.Console/      # Consoleappen (valfritt att ändra för felsökning)

CarMechanicApp.Tests/          # Testprojektet - ÄNDRA INTE
```

## Att fixa: Buggar

1. **Bugg i `GetPartUsageReport`** - GetPartUsageReport subtracts part quantity instead of adding when the part already exists in the report
2. **Bugg i `HireMechanic`** - HireMechanic sets HireDate to MinValue instead of current time
3. **Bugg i `ReassignMechanic`** - ReassignMechanic does not persist the mechanic change (Update call removed)
4. **Bugg i `Search`** [Infrastructure] - PartRepository Search requires both name AND part number to match
5. **Bugg i `ReassignMechanic`** - ReassignMechanic uses AND instead of OR in status check so it never prevents reassignment
6. **Bugg i `GetVehiclesByType`** - GetVehiclesByType returns all vehicles unfiltered
7. **Bugg i `GetPartUsageReport`** - GetPartUsageReport does not filter by date range (date conditions removed from Where clause)
8. **Bugg i `CreateInvoice`** - CreateInvoice does not apply discount to total amount

## Att implementera: Saknade metoder

1. **`GetMechanicRevenue`** - Implement GetMechanicRevenue that calculates total revenue from completed work orders
2. **`GetPartsByCategory`** - Implement GetPartsByCategory that returns parts filtered by category
3. **`GetInvoicesByCustomer`** - Implement GetInvoicesByCustomer that returns invoices for a specific customer
4. **`GetMechanicFullName`** - Implement GetMechanicFullName that returns the full name of a mechanic

## Verifiering

När du är klar, kör testerna:

```bash
dotnet test CarMechanicApp.Tests/CarMechanic.Tests.csproj
```

**Alla tester gröna = Godkänd uppgift!**
