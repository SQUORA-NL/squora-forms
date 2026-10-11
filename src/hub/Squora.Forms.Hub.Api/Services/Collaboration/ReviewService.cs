using Squora.Forms.Hub.Api.Models.Collaboration;
using MongoDB.Driver;

namespace Squora.Forms.Hub.Api.Services.Collaboration;

public class ReviewService
{
    private readonly IMongoCollection<ReviewRequest> _reviews;

    public ReviewService(IMongoDatabase database)
    {
        _reviews = database.GetCollection<ReviewRequest>("review_requests");
    }

    public async Task<ReviewRequest> CreateAsync(string workspaceId, string definitionId,
        int definitionVersion, string requestedByUserId, List<string> reviewerIds)
    {
        ValidateCreate(requestedByUserId, reviewerIds);
        var review = BuildReviewRequest(workspaceId, definitionId, definitionVersion, requestedByUserId, reviewerIds);
        await _reviews.InsertOneAsync(review);
        return review;
    }

    public async Task<ReviewRequest> ApproveAsync(string reviewId, string reviewerUserId, string note)
    {
        var review = await GetReviewOrThrowAsync(reviewId);
        ValidateReviewer(review, reviewerUserId);
        review.AuditLog.Add(new ReviewEvent { ActorUserId = reviewerUserId, Type = "approved", Note = note });
        if (AllReviewersApproved(review))
            review.Status = "approved";
        await _reviews.ReplaceOneAsync(r => r.Id == reviewId, review);
        return review;
    }

    public async Task<ReviewRequest> RequestChangesAsync(string reviewId, string reviewerUserId, string note)
    {
        var review = await GetReviewOrThrowAsync(reviewId);
        review.Status = "changes-requested";
        review.AuditLog.Add(new ReviewEvent { ActorUserId = reviewerUserId, Type = "changes-requested", Note = note });
        await _reviews.ReplaceOneAsync(r => r.Id == reviewId, review);
        return review;
    }

    public async Task<ReviewRequest?> GetLatestForDefinitionAsync(string definitionId)
    {
        return await _reviews.Find(r => r.DefinitionId == definitionId)
            .SortByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task InvalidateByDefinitionAsync(string definitionId)
    {
        var filter = Builders<ReviewRequest>.Filter.Where(r => r.DefinitionId == definitionId && r.Status == "approved");
        await _reviews.UpdateManyAsync(filter, Builders<ReviewRequest>.Update.Set(r => r.Status, "pending"));
    }

    private async Task<ReviewRequest> GetReviewOrThrowAsync(string reviewId)
    {
        return await _reviews.Find(r => r.Id == reviewId).FirstOrDefaultAsync()
            ?? throw new InvalidOperationException($"Review {reviewId} not found.");
    }

    private static void ValidateCreate(string requestedBy, List<string> reviewerIds)
    {
        if (reviewerIds.Count == 0)
            throw new InvalidOperationException("At least one reviewer must be assigned.");
        if (reviewerIds.Contains(requestedBy))
            throw new InvalidOperationException("Cannot self-approve: requester cannot be a reviewer.");
    }

    private static void ValidateReviewer(ReviewRequest review, string reviewerUserId)
    {
        if (!review.AssignedReviewerIds.Contains(reviewerUserId))
            throw new InvalidOperationException("Only assigned reviewers can approve.");
    }

    private static bool AllReviewersApproved(ReviewRequest review)
    {
        var approved = review.AuditLog
            .Where(e => e.Type == "approved")
            .Select(e => e.ActorUserId)
            .ToHashSet();
        return review.AssignedReviewerIds.All(approved.Contains);
    }

    private static ReviewRequest BuildReviewRequest(string workspaceId, string definitionId,
        int version, string requestedBy, List<string> reviewerIds)
    {
        return new ReviewRequest
        {
            WorkspaceId = workspaceId,
            DefinitionId = definitionId,
            DefinitionVersion = version,
            RequestedByUserId = requestedBy,
            AssignedReviewerIds = reviewerIds,
            AuditLog = { new ReviewEvent { ActorUserId = requestedBy, Type = "submitted" } }
        };
    }
}
