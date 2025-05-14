namespace DT.Orders.DTOs;

public record OrderRequest(Guid ProductId, int Quantity, decimal TotalPrice); 
