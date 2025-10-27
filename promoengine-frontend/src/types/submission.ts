export interface SubmissionRequest {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
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

export interface SubmissionFormData extends SubmissionRequest {
  confirmEmail: string;
}