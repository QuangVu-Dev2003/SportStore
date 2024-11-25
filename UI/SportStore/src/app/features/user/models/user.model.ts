export class User {
    id: string;
    userName: string;
    email: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    address: string;

    constructor(data?: Partial<User>) {
        this.id = data?.id || '';
        this.userName = data?.userName || '';
        this.email = data?.email || '';
        this.firstName = data?.firstName || '';
        this.lastName = data?.lastName || '';
        this.phoneNumber = data?.phoneNumber || '';
        this.address = data?.address || '';
    }
}
