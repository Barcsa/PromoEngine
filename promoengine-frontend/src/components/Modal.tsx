import React from "react";

interface ModalProps {
    isOpen: boolean;
    message: string;
    onClose: () => void;
}

const Modal: React.FC<ModalProps> = ({ isOpen, message, onClose }) => {
    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 bg-black/60 flex justify-center items-center z-50">
            <div className="bg-gray-900 p-6 rounded-lg shadow-lg max-w-sm text-center">
                <p className="text-white mb-4">{message}</p>
                <button
                    onClick={onClose}
                    className="px-4 py-2 bg-indigo-500 hover:bg-indigo-600 rounded-md text-white"
                >
                    OK
                </button>
            </div>
        </div>
    );
};

export default Modal;
