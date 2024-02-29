﻿namespace StellarWallet.Application.Dtos.Responses
{
    public class UserDto(int Id, string name, string lastName, string email, string publicKey)
    {
        public int Id { get; set; } = Id;
        public string Name { get; set; } = name; 
        public string LastName { get; set; } = lastName;
        public string Email { get; set; } = email;
        public string PublicKey { get; set; } = publicKey;
    }
}
