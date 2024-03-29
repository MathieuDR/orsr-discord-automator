﻿using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace DiscordBot.Common.Models.Data.Base;

public class BaseModel {
	protected Dictionary<string, string> ValidationDictionary = new();

	public BaseModel() => CreatedOn = DateTime.Now;

	public BaseModel(DiscordUserId userId) : this() => CreatedByDiscordId = userId;

	// ReSharper disable once InconsistentNaming
	[BsonId]
	public ObjectId _id { get; set; }

	public DiscordUserId CreatedByDiscordId { get; set; }
	public DateTime CreatedOn { get; set; }

	public virtual void IsValid() {
		if (CreatedByDiscordId != DiscordUserId.Empty) {
			ValidationDictionary.Add(nameof(CreatedByDiscordId), "created by Id must be higher then 0");
		}

		if (ValidationDictionary.Any()) {
			throw new ValidationException(ValidationDictionary.Select(x => $"{x.Key} - {x.Value}")
				.Aggregate((i, j) => i + ", " + j));
		}
	}

	public virtual Dictionary<string, string> ToDictionary() {
		var t = GetType();
		var props = t.GetProperties();
		var dict = new Dictionary<string, string>();

		foreach (var prp in props) {
			if (!IsPrimitiveOrString(prp.PropertyType)) {
				continue;
			}

			var value = prp.GetValue(this, new object[] { });
			var friendlyValue = "not set";

			if (value != null) {
				friendlyValue = value.ToString();
			}

			dict.Add(prp.Name, friendlyValue);
		}

		return dict;
	}

	private bool IsPrimitiveOrString(Type type) =>
		type.IsPrimitive
		|| type.Equals(typeof(string));
}
