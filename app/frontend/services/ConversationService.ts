import { Conversation, Message } from '@/types';
import { ApiService } from './ApiService';

const CONVERSATION_API_URL = '/conversations';

export class ConversationService {

    static async create(): Promise<Conversation> {
        const response = await ApiService.post<Conversation>(CONVERSATION_API_URL, {});

        if (response.error) {
            throw new Error(response.error);
        }

        return response.data!;
    }

    static async getById(conversationId: string): Promise<Conversation> {
        const response = await ApiService.get<Conversation>(`${CONVERSATION_API_URL}/${conversationId}`);

        if (response.error) {
            throw new Error(response.error);
        }

        return response.data!;
    }

    static async getAll(): Promise<Conversation[]> {
        const response = await ApiService.get<Conversation[]>(CONVERSATION_API_URL);

        if (response.error) {
            throw new Error(response.error);
        }

        return response.data!;
    }

    static async addMessage(conversationId: string, message: Omit<Message, 'id' | 'createdAt'>): Promise<Conversation> {
        const response = await ApiService.post<Conversation>(`${CONVERSATION_API_URL}/${conversationId}/messages`, message);

        if (response.error) {
            throw new Error(response.error);
        }

        return response.data!;
    }

    static async getMessages(conversationId: string): Promise<Message[]> {
        const response = await ApiService.get<Message[]>(`${CONVERSATION_API_URL}/${conversationId}/messages`);

        if (response.error) {
            throw new Error(response.error);
        }

        return response.data!;
    }
}
