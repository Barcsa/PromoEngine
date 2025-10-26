export interface SubmissionRequest {
    firstName: string;
    lastName: string;
    email: string;
    promoCode: string;
    acceptedPrivacyPolicy: boolean;
    acceptedGameRules: boolean;
}

export interface SubmissionResponse {
    success: boolean;
    message: string;
    isWinner: boolean;
    prizeType?: string | null;
}
