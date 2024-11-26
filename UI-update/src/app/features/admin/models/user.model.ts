export interface RefreshToken {
    id: number;
    userId: string;
    token: string;
    jwtId: string;
    isRevoked: boolean;
    dateAdded: Date;
    dateExpire: Date;
}

export interface AppUser {
    id: string;
    userName: string;
    email: string;
    firstName?: string;
    lastName?: string;
    phoneNumber?: string;
    address?: string;
    accountType: string;
    memberSince: Date;
    lastLogin?: Date;
    failedLoginAttempts: number;
    lockoutEnd?: Date;
    isDeleted: boolean;
    deletionDate?: Date;
    refreshTokens: RefreshToken[];
}
