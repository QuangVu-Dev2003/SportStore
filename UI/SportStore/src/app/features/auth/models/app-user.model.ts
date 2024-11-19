export interface RefreshToken {
    token: string;
    expiresAt: Date;
}

export interface AppUser {
    id: string;
    userName: string;
    email: string;
    refreshTokens: RefreshToken[];
    failedLoginAttempts: number;
    lockoutEnd?: Date;
}
