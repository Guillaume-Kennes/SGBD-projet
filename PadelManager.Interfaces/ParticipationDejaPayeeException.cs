namespace PadelManager.Interfaces;

// Levée par IMatchRepository.PayerParticipationAsync lorsque la participation a déjà un
// paiement (filet de sécurité redondant avec UQ_PAIEMENT_participationId).
public class ParticipationDejaPayeeException : Exception {
}
