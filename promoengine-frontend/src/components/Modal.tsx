import React from "react";

interface ModalProps {
    isOpen: boolean;
    message: string;
    onClose: () => void;
    success?: boolean;
    isWinner?: boolean;
}

const Modal: React.FC<ModalProps> = ({ isOpen, message, onClose, success, isWinner }) => {
    if (!isOpen) return null;

    const isError = success === false;
    const isWin = isWinner === true;

    const bgGradient = isWin
        ? "from-[#1c522f]/95 to-[#1f6b3a]/95"
        : isError
            ? "from-[#3b1c1c]/95 to-[#521f1f]/95"
            : success && !isWin
                ? "from-[#e9f1f6]/95 to-[#dbe8f1]/95"
                : "from-[#1b4054]/95 to-[#153247]/95";

    const borderColor = isWin
        ? "border-[#2eb872]"
        : isError
            ? "border-[#8b2e2e]"
            : success && !isWin
                ? "border-[#b5c9d8]"
                : "border-[#3d556a]";

    const buttonColor = isWin
        ? "bg-[#37d97b] hover:bg-[#44f089] text-[#073219]"
        : isError
            ? "bg-[#e24b4b] hover:bg-[#f35d5d] text-[#0e2a3b]"
            : success && !isWin
                ? "bg-[#0e2a3b] hover:bg-[#153247] text-white"
                : "bg-[#4da0db] hover:bg-[#63b4eb] text-[#0e2a3b]";


    const buttonText = isWin ? "Szuper!" : isError ? "Újrapróbálom" : "Értem";

    return (
        <div className="fixed inset-0 bg-[#0b2230]/80 backdrop-blur-sm flex justify-center items-center z-50">
            <div
                className={`bg-gradient-to-b ${bgGradient} border ${borderColor} shadow-2xl rounded-2xl p-6 md:p-8 max-w-sm w-[90%] text-center`}
            >
                <p className="mb-6 text-base leading-relaxed text-center">
                    {isWin && message.startsWith("Nyertél, Bajnok!") ? (
                        <>
                            <span className="block text-2xl font-extrabold text-green-100 mb-2 drop-shadow-[0_0_6px_rgba(0,255,150,0.4)]">
                                Nyertél, Bajnok!
                            </span>
                            <span className="block text-green-50 text-base">
                                Megnyerted a napi nyereményt!
                            </span>
                        </>
                    ) : !isWin && success && message.startsWith("Most nem volt szerencséd, Bajnok!") ? (
                        <>
                            <span className="block text-xl font-bold text-[#0e2a3b] mb-1">
                                Most nem volt szerencséd, Bajnok!
                            </span>
                            <span className="block text-gray-700 text-sm">
                                A nagy győzelemhez sok küzdelem kell. Próbáld újra!
                            </span>
                        </>
                    ) : (
                        <span
                            className={
                                isError
                                    ? "text-red-200"
                                    : isWin
                                        ? "text-green-100 font-semibold text-lg"
                                        : "text-[#d2ecff]"
                            }
                        >
                            {message}
                        </span>
                    )}
                </p>

                <button
                    onClick={onClose}
                    className={`px-6 py-2 rounded-full ${buttonColor} font-semibold transition-all shadow-md`}
                >
                    {buttonText}
                </button>
            </div>
        </div>
    );
};

export default Modal;