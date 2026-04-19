import bcrypt from 'bcrypt';

const saltRounds = Number(process.env.SALT_ROUNDS) || 10;

export async function hashPassword(password: string) {
    try {
        console.log(password);
        return await bcrypt.hash(password, saltRounds);
    } catch (error) {
        console.log(error);
        throw new Error('Hash password is error');
    }
};

export async function comparePassword(password: string, hashedPassword: string) {
    try {
        return await bcrypt.compare(password, hashedPassword);
    } catch (error) {
        throw new Error('Compare password is error');
    }
};